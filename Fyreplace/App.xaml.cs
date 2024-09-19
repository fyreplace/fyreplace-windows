using CommunityToolkit.WinUI;
using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Data.Preferences;
using Fyreplace.Data.Secrets;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.ViewModels;
using Fyreplace.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using Microsoft.Windows.AppLifecycle;
using Polly;
using Polly.Retry;
using Sentry;
using Sentry.Protocol;
using System;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

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
#if DEBUG
                    options.TracesSampleRate = 1;
                    options.ProfilesSampleRate = 1;
                    options.EnableSpotlight = true;
#endif
                });

                UnhandledException += OnUnhandledException;
            }

            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var instances = AppInstance.GetInstances();
            var currentInstance = AppInstance.GetCurrent();
            var isSingleInstance = instances.Count == 1;
            var protocolActivatedArgs = currentInstance.GetActivatedEventArgs().Data as ProtocolActivatedEventArgs;
            AppInstance.FindOrRegisterForKey(Guid.NewGuid().ToString());
            var shouldExit = false;

            switch (protocolActivatedArgs?.Uri.AbsolutePath)
            {
                case "/login":
                case "/register":
                    if (isSingleInstance)
                    {
                        _ = CompleteConnectionAsync(protocolActivatedArgs!);
                    }
                    else
                    {
                        foreach (var instance in instances)
                        {
                            _ = instance.RedirectActivationToAsync(currentInstance.GetActivatedEventArgs());
                        }

                        shouldExit = true;
                    }

                    break;
            }

            if (shouldExit)
            {
                Exit();
            }
            else
            {
                GetService<MainWindow>().Activate();
                currentInstance.Activated += AppInstance_Activated;
            }
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddHostedService<TokenRefreshService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ISecrets, PasswordVaultSecrets>();
            services.AddSingleton<IEventBus, EventBus>();
            services.AddResiliencePipeline(typeof(RequestHeadersHandler), MakeResiliencePipeline);
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
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
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
            var resilience = provider.GetRequiredKeyedService<ResiliencePipeline>(typeof(RequestHeadersHandler));
            var preferences = provider.GetRequiredService<IPreferences>();
            var secrets = provider.GetRequiredService<ISecrets>();
            var api = provider.GetRequiredService<BuildInfo>().Api;
            var url = api.ForEnvironment(preferences.Connection_Environment);
            var client = new HttpClient(new RequestHeadersHandler(secrets, resilience));
            return new ApiClient(url.ToString(), client);
        }

        private void AppInstance_Activated(object? sender, AppActivationArguments e)
        {
            var protocolActivatedArgs = e.Data as ProtocolActivatedEventArgs;

            if (protocolActivatedArgs != null)
            {
                var window = GetService<MainWindow>();
                window.DispatcherQueue.EnqueueAsync(
                    async () =>
                    {
                        window.Show();
                        await CompleteConnectionAsync(protocolActivatedArgs);
                    },
                    DispatcherQueuePriority.High
                );
            }
        }

        private static Task CompleteConnectionAsync(ProtocolActivatedEventArgs protocolActivatedArgs) => GetService<MainWindowViewModel>().CompleteConnectionAsync(protocolActivatedArgs.Uri.Fragment.Replace("#", ""));
    }

    class RequestHeadersHandler(ISecrets secrets, ResiliencePipeline resilience) : DelegatingHandler(new SentryHttpMessageHandler())
    {
        public ResiliencePipeline resilience = resilience;

        private static readonly string headerName = "X-Request-Id";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (secrets.Token != string.Empty)
            {
                request.Headers.Authorization = new("Bearer", secrets.Token);
            }

            if (!request.Headers.Contains(headerName))
            {
                request.Headers.Add(headerName, Guid.NewGuid().ToString());
            }

            return await resilience.ExecuteAsync(async (token) => await base.SendAsync(request, token), cancellationToken);
        }
    }
}
