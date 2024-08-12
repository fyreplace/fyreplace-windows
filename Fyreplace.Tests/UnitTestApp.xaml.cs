using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.Tests.Data;
using Fyreplace.Tests.Events;
using Fyreplace.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestPlatform.TestExecutor;
using System;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.Tests
{
    public partial class UnitTestApp : AppBase
    {
        public UnitTestApp() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs args) => UnitTestClient.Run(System.Environment.CommandLine);

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.RemoveAll<ISettings>();
            services.RemoveAll<IEventBus>();
            services.RemoveAll<IApiClient>();
            services.AddSingleton<ISettings, MemorySettings>();
            services.AddSingleton<IEventBus, StoringEventBus>();

            foreach (var environment in Enum.GetValues<Environment>())
            {
                services.AddKeyedTransient<IApiClient>(environment, (_, _) => new FakeApiClient());
            }
        }
    }
}
