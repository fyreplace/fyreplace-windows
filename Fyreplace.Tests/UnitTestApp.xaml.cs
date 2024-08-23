using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.Tests.Data.Preferences;
using Fyreplace.Tests.Data.Secrets;
using Fyreplace.Tests.Events;
using Fyreplace.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestPlatform.TestExecutor;

namespace Fyreplace.Tests
{
    public partial class UnitTestApp : AppBase
    {
        public UnitTestApp() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs args) => UnitTestClient.Run(System.Environment.CommandLine);

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IPreferences, MemoryPreferences>();
            services.AddSingleton<ISecrets, MemorySecrets>();
            services.AddSingleton<IEventBus, StoringEventBus>();
            services.AddTransient<IApiClient, FakeApiClient>();
        }
    }
}
