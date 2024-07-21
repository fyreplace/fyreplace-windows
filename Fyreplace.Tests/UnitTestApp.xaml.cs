using Fyreplace.Data;
using Fyreplace.Tests.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Environment = System.Environment;

namespace Fyreplace.Tests
{
    public partial class UnitTestApp : AppBase
    {
        public UnitTestApp() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();
            var window = GetService<MainWindow>();
            window.Activate();
            UITestMethodAttribute.DispatcherQueue = window.DispatcherQueue;
            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.RemoveAll<ISettings>();
            services.AddSingleton<ISettings, MemorySettings>();
        }
    }
}
