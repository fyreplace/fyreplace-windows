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
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
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
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();
            var info = host.Services.GetRequiredService<BuildInfo>();

            if (!info.App.SelfContained && DeploymentManager.GetStatus().Status != DeploymentStatus.Ok)
            {
                DeploymentManager.Initialize();
            }

            if (!string.IsNullOrEmpty(info.Sentry.Dsn))
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
            host.Start();
            var instances = AppInstance.GetInstances();
            var currentInstance = AppInstance.GetCurrent();
            var isSingleInstance = instances.Count == 1;
            var protocolActivatedArgs = currentInstance.GetActivatedEventArgs().Data as ProtocolActivatedEventArgs;
            AppInstance.FindOrRegisterForKey(Guid.NewGuid().ToString());

            if (isSingleInstance)
            {
                if (protocolActivatedArgs != null)
                {
                    _ = HandleActivatedArgs(protocolActivatedArgs);
                }

                currentInstance.Activated += AppInstance_Activated;
                host.Services.GetRequiredService<MainWindow>().Activate();
            }
            else
            {
                foreach (var instance in instances)
                {
                    _ = instance.RedirectActivationToAsync(currentInstance.GetActivatedEventArgs());
                }

                Exit();
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

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddResiliencePipeline(typeof(RequestHeadersHandler), MakeResiliencePipeline);
            services.AddHostedService<TokenRefreshService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<BuildInfo>();
            services.AddSingleton<ISecrets, PasswordVaultSecrets>();
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IStringsService, StringsService>();
            services.AddSingleton<IImageFileService, DelegatingImageFileService>();
            services.AddTransient(MakeApiClient);

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<AccountViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AvatarViewModel>();

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
                var window = host.Services.GetRequiredService<MainWindow>();
                window.DispatcherQueue.EnqueueAsync(
                    async () =>
                    {
                        window.Show();
                        await HandleActivatedArgs(protocolActivatedArgs);
                    },
                    DispatcherQueuePriority.High
                );
            }
        }

        private async Task HandleActivatedArgs(ProtocolActivatedEventArgs protocolActivatedArgs)
        {
            switch (protocolActivatedArgs.Uri.AbsolutePath)
            {
                case "/login":
                case "/register":
                    await CompleteUserConnectionAsync(protocolActivatedArgs);
                    break;

                case "/settings/emails":
                    await CompleteEmailVerificationAsync(protocolActivatedArgs);
                    break;
            }
        }

        private Task CompleteUserConnectionAsync(ProtocolActivatedEventArgs protocolActivatedArgs) => host.Services.GetRequiredService<MainWindowViewModel>().CompleteUserConnectionAsync(protocolActivatedArgs.Uri.Fragment.Replace("#", string.Empty));

        private Task CompleteEmailVerificationAsync(ProtocolActivatedEventArgs protocolActivatedArgs)
        {
            var fragmentParts = protocolActivatedArgs.Uri.Fragment.Replace("#", string.Empty).Split(":");
            return host.Services.GetRequiredService<MainWindowViewModel>().CompleteEmailVerificationAsync(fragmentParts[0], fragmentParts[1]);
        }

        public static T GetService<T>() where T : notnull => ((App)Current).host.Services.GetRequiredService<T>();
    }

    partial class RequestHeadersHandler(ISecrets secrets, ResiliencePipeline resilience) : DelegatingHandler(new SentryHttpMessageHandler())
    {
        public ResiliencePipeline resilience = resilience;

        private static readonly string headerName = "X-Request-Id";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(secrets.Token))
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
