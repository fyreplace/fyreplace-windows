using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Data.Preferences;
using Fyreplace.Data.Secrets;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using Polly;
using Polly.Retry;
using Sentry;
using Sentry.Protocol;
using System;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Fyreplace
{
    public partial class App : AppBase
    {
        public App()
        {
            var info = GetService<BuildInfo>();

            if (!info.App.SelfContained && DeploymentManager.GetStatus().Status != DeploymentStatus.Ok)
            {
                DeploymentManager.Initialize();
            }

            if (info.Sentry.Dsn != string.Empty)
            {
                SentrySdk.Init(options =>
                {
                    options.Dsn = info.Sentry.Dsn;
                    options.Environment = info.Sentry.Environment;
                    options.AutoSessionTracking = true;
                    options.IsGlobalModeEnabled = true;
                    options.CaptureFailedRequests = true;
                    options.DisableWinUiUnhandledExceptionIntegration();
                });

                UnhandledException += OnUnhandledException;
            }

            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            GetService<MainWindow>().Activate();
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddHostedService<TokenRefreshService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ISecrets, PasswordVaultSecrets>();
            services.AddSingleton<IEventBus, EventBus>();
            services.AddResiliencePipeline(typeof(RequestIdHandler), MakeResiliencePipeline);
            services.AddTransient(MakeApiClient);

            var info = services.BuildServiceProvider().GetRequiredService<BuildInfo>();

            if (info.App.SelfContained)
            {
                services.AddSingleton<IPreferences, RegistryPreferences>();
            }
            else
            {
                services.AddSingleton<IPreferences, LocalSettingsPreferences>();
            }
        }

        [SecurityCritical]
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;

            if (exception == null)
            {
                return;
            }

            exception.Data[Mechanism.HandledKey] = false;
            exception.Data[Mechanism.MechanismKey] = "Application.UnhandledException";
            SentrySdk.CaptureException(exception);
            SentrySdk.FlushAsync(TimeSpan.FromSeconds(3)).GetAwaiter().GetResult();
        }

        private void MakeResiliencePipeline(ResiliencePipelineBuilder builder) => builder
            .AddRetry(new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                BackoffType = DelayBackoffType.Exponential,
            });

        private IApiClient MakeApiClient(IServiceProvider provider)
        {
            var resilience = provider.GetRequiredKeyedService<ResiliencePipeline>(typeof(RequestIdHandler));
            var preferences = provider.GetRequiredService<IPreferences>();
            var secrets = provider.GetRequiredService<ISecrets>();
            var api = provider.GetRequiredService<BuildInfo>().Api;
            var url = api.ForEnvironment(preferences.Connection_Environment);
            var client = new HttpClient(new RequestIdHandler(resilience));

            if (secrets.Token != string.Empty)
            {
                client.DefaultRequestHeaders.Authorization = new("Bearer", secrets.Token);
            }

            return new ApiClient(url.ToString(), client);
        }
    }

    class RequestIdHandler(ResiliencePipeline resilience) : DelegatingHandler(new HttpClientHandler())
    {
        public ResiliencePipeline resilience = resilience;

        private static readonly string headerName = "X-Request-Id";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(headerName))
            {
                request.Headers.Add(headerName, Guid.NewGuid().ToString());
            }

            return await resilience.ExecuteAsync(async (token) => await base.SendAsync(request, token), cancellationToken);
        }
    }
}
