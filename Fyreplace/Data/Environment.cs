using Microsoft.Windows.ApplicationModel.Resources;

#if !DEBUG
using Fyreplace.Config;
#endif

namespace Fyreplace.Data
{
    public enum Environment
    {
        Main,
        Dev,
        Local
    }

    public static class EnvironmentExtensions
    {
        public static Environment Default =>
#if DEBUG
                Environment.Local;
#else
                AppBase.GetService<BuildInfo>().Sentry.Environment == "dev" ? Environment.Dev : Environment.Main;
#endif


        public static string Description(this Environment environment)
        {
            var resourceLoader = new ResourceLoader();
            var description = resourceLoader.GetString($"Environment_{environment}");

            if (environment == EnvironmentExtensions.Default)
            {
                description += " " + resourceLoader.GetString("Environment_DefaultSuffix");
            }

            return description;
        }
    }
}
