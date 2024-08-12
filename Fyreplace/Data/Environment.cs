using Microsoft.Windows.ApplicationModel.Resources;

namespace Fyreplace.Data
{
    public enum Environment
    {
        Main,
        Dev,
#if DEBUG
        Local
#endif
    }

    public static class EnvironmentExtensions
    {
        public static Environment Default =>
#if DEBUG
                Environment.Local;
#else
                AppBase.GetService<Fyreplace.Config.BuildInfo>().Sentry.Environment == "dev" ? Environment.Dev : Environment.Main;
#endif

        public static string Description(this Environment environment)
        {
            var resourceLoader = new ResourceLoader();
            var description = resourceLoader.GetString($"Environment_{environment}");

            if (environment == Default)
            {
                description += " " + resourceLoader.GetString("Environment_DefaultSuffix");
            }

            return description;
        }
    }
}
