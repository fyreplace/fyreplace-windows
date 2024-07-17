using Fyreplace.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using Sentry;
using Sentry.Protocol;
using System;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Fyreplace
{
    public partial class App : Application
    {
        private Window? window;

        public App()
        {
            if (!Config.App.SelfContained && DeploymentManager.GetStatus().Status != DeploymentStatus.Ok)
            {
                DeploymentManager.Initialize();
            }

            if (Config.Sentry.Dsn != "")
            {
                SentrySdk.Init(options =>
                {
                    options.Dsn = Config.Sentry.Dsn;
                    options.Environment = Config.Sentry.Environment;
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
            window = new MainWindow();
            window.Activate();
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
    }
}
