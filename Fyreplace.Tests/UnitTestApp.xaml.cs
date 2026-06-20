using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.Tests.Data.Preferences;
using Fyreplace.Tests.Data.Secrets;
using Fyreplace.Tests.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestPlatform.TestExecutor;

namespace Fyreplace.Tests
{
    public partial class UnitTestApp : Application
    {
        private readonly IHost host;

        public UnitTestApp()
        {
            host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            host.Start();
            UnitTestClient.Run(System.Environment.CommandLine);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BuildInfo>();
            services.AddSingleton<IPreferences, MemoryPreferences>();
            services.AddSingleton<ISecrets, MemorySecrets>();
            services.AddSingleton<IEventBus, StoringEventBus>();
            services.AddSingleton<IStringsService, FakeStringsService>();
            services.AddSingleton<IImageFileService, FakeImageFileService>();
            services.AddTransient<IApiClient, FakeApiClient>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<AccountViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AvatarViewModel>();
        }

        public static T GetService<T>() where T : notnull => ((UnitTestApp)Current).host.Services.GetRequiredService<T>();
    }
}
