using static Fyreplace.Helpers.Metadata;
using System;
using System.Reflection;
using System.Diagnostics;

namespace Fyreplace.Config
{
    public class BuildInfo
    {
        public readonly Version Version = new();
        public readonly App App = new();
        public readonly Sentry Sentry = new();
    }

    public class Version
    {
        public string Main => Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        public string Informational => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion!;
    }

    public class App
    {
        public bool SelfContained => GetCustomAttribute("App.SDKSelfContained")?.ToLower() == "true";

        public class Api
        {
            public Uri Main => new(GetCustomAttribute("Api.MainUrl")!);
            public Uri Dev => new(GetCustomAttribute("Api.DevUrl")!);
            public Uri? Local => TryMakeUri(GetCustomAttribute("Api.LocalUrl"));
        }
    }

    public class Sentry
    {
        public string? Dsn => GetCustomAttribute("Sentry.Dsn");
        public string? Environment => GetCustomAttribute("Sentry.Environment");
    }
}
