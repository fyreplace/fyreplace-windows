using Fyreplace.Config;
using Fyreplace.ViewModels;
using Fyreplace.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace Fyreplace
{
    public abstract class AppBase : Application
    {
        private readonly IHost host;

        public AppBase() => host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();

        protected override void OnLaunched(LaunchActivatedEventArgs args) => host.Start();

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BuildInfo>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<AccountViewModel>();
        }

        public static T GetService<T>() where T : notnull => ((AppBase)Current).host.Services.GetRequiredService<T>();

        public static IServiceScope CreateServiceScope() => ((AppBase)Current).host.Services.CreateScope();
    }
}
