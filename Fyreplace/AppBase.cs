using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.ViewModels;
using Fyreplace.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.Net.Http;
using Environment = Fyreplace.Data.Environment;

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
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<MainWindow>();

            var info = services.BuildServiceProvider().GetRequiredService<BuildInfo>();

            if (info.App.SelfContained)
            {
                services.AddSingleton<ISettings, RegistrySettings>();
            }
            else
            {
                services.AddSingleton<ISettings, LocalSettings>();
            }

            foreach (var environment in Enum.GetValues<Environment>())
            {
                services.AddKeyedTransient(environment, MakeApiClient);
            }
        }

        private IApiClient MakeApiClient(IServiceProvider provider, object? key)
        {
            var environment = (Environment)key!;
            var api = provider.GetRequiredService<BuildInfo>().Api;
            var url = api.ForEnvironment(environment);
            return new ApiClient(url.ToString(), new HttpClient());

        }

        public static T GetService<T>() where T : notnull => ((AppBase)Current).host.Services.GetRequiredService<T>();

        public static T GetService<T>(object key) where T : notnull => ((AppBase)Current).host.Services.GetRequiredKeyedService<T>(key);

        public static IServiceScope CreateServiceScope() => ((AppBase)Current).host.Services.CreateScope();
    }
}
