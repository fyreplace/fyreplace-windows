using Microsoft.UI.Xaml;
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
            var dsn = GetCustomAttribute("Sentry.Dsn");

            if (dsn != "")
            {
                SentrySdk.Init(options =>
                {
                    options.Dsn = dsn;
                    options.Environment = GetCustomAttribute("Sentry.Environment");
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

        private static string GetCustomAttribute(string key) => Assembly.GetExecutingAssembly()
                .GetCustomAttributes(false)
                .OfType<AssemblyMetadataAttribute>()
                .Where(a => a.Key == key)
                .Select(a => a.Value)
                .FirstOrDefault() ?? "";
    }
}
