using System;
using System.Linq;
using System.Reflection;

namespace Fyreplace.Helpers
{
    public static class Config
    {
        private static string? GetCustomAttribute(string key) => Assembly.GetExecutingAssembly()
                .GetCustomAttributes(false)
                .OfType<AssemblyMetadataAttribute>()
                .Where(a => a.Key == key)
                .Select(a => a.Value)
                .FirstOrDefault();

        private static Uri? TryMakeUri(string? uri) => uri != null && uri != "" ? new Uri(uri) : null;

        public static class App
        {
            public static bool SelfContained => GetCustomAttribute("App.SDKSelfContained")?.ToLower() == "true";

            public static class Api
            {
                public static Uri Main => new(GetCustomAttribute("Api.MainUrl")!);
                public static Uri Dev => new(GetCustomAttribute("Api.DevUrl")!);
                public static Uri? Local => TryMakeUri(GetCustomAttribute("Api.LocalUrl"));
            }
        }

        public static class Sentry
        {
            public static string? Dsn => GetCustomAttribute("Sentry.Dsn");
            public static string? Environment => GetCustomAttribute("Sentry.Environment");
        }
    }
}
