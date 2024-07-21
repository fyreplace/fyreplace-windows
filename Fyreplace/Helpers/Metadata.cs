using System;
using System.Linq;
using System.Reflection;

namespace Fyreplace.Helpers
{
    public static class Metadata
    {
        public static string? GetCustomAttribute(string key) => Assembly.GetExecutingAssembly()
                .GetCustomAttributes(false)
                .OfType<AssemblyMetadataAttribute>()
                .Where(a => a.Key == key)
                .Select(a => a.Value)
                .FirstOrDefault();

        public static Uri? TryMakeUri(string? uri) => uri != null && uri != "" ? new Uri(uri) : null;
    }
}
