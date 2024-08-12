using static Fyreplace.Config.Metadata;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.Config
{
    public sealed class BuildInfo
    {
        public readonly Version Version = new();
        public readonly App App = new();
        public readonly Api Api = new();
        public readonly Sentry Sentry = new();
    }

    public sealed class Version
    {
        public readonly string Main = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        public readonly string Informational = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion!;
    }

    public sealed class App
    {
        public readonly bool SelfContained = GetCustomAttribute("App.SDKSelfContained")?.ToLower() == "true";
    }

    public sealed class Api
    {
        public readonly Uri Main = new(GetCustomAttribute("Api.MainUrl")!);
        public readonly Uri Dev = new(GetCustomAttribute("Api.DevUrl")!);
#if DEBUG
        public readonly Uri Local = new(GetCustomAttribute("Api.LocalUrl")!);
#endif

        public Uri ForEnvironment(Environment environment) => environment switch
        {
            Environment.Main => Main,
            Environment.Dev => Dev,
#if DEBUG
            Environment.Local => Local,
#endif
            _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, null)
        };
    }

    public sealed class Sentry
    {
        public readonly string? Dsn = GetCustomAttribute("Sentry.Dsn");
        public readonly string? Environment = GetCustomAttribute("Sentry.Environment");
    }

    public static class Metadata
    {
        public static string? GetCustomAttribute(string key) => Assembly.GetExecutingAssembly()
                .GetCustomAttributes(false)
                .OfType<AssemblyMetadataAttribute>()
                .Where(a => a.Key == key)
                .Select(a => a.Value)
                .FirstOrDefault();
    }
}
