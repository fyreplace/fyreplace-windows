using Fyreplace.Config;
using Fyreplace.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace Fyreplace
{
    public abstract class AppBase : Application
    {
        private readonly IHost host;

        public AppBase() => host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();

        protected override void OnLaunched(LaunchActivatedEventArgs args) => host.Start();

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BuildInfo>();
            var info = services.BuildServiceProvider().GetRequiredService<BuildInfo>();

            if (info.App.SelfContained)
            {
                services.AddSingleton<ISettings, RegistrySettings>();
            }
            else
            {
                services.AddSingleton<ISettings, LocalSettings>();
            }

            services.AddSingleton<MainWindow>();
        }

        public static T GetService<T>() where T : notnull => ((AppBase)Current).host.Services.GetRequiredService<T>();
    }
}
