using Fyreplace.Config;
using Fyreplace.Services;

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
        public static Environment Default(BuildInfo buildInfo) =>
#if DEBUG
                Environment.Local;
#else
                buildInfo.Sentry.Environment == "dev" ? Environment.Dev : Environment.Main;
#endif

        public static string Description(this Environment environment, BuildInfo buildInfo, IStringsService stringsService)
        {
            var description = stringsService.GetString($"Environment_{environment}");

            if (environment == Default(buildInfo))
            {
                description += " " + stringsService.GetString("Environment_DefaultSuffix");
            }

            return description;
        }
    }
}
