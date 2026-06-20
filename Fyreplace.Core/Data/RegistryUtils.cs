using Fyreplace.Config;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fyreplace.Data
{
    public static class RegistryUtils
    {
        public static RegistryKey GetRegistryKey(IEnumerable<string> path, BuildInfo buildInfo)
        {
            var registryKey = Registry.CurrentUser
                .CreateSubKey("Software")
                .CreateSubKey(buildInfo.App.Name);

            foreach (var segment in path)
            {
                registryKey = registryKey.CreateSubKey(segment);
            }

            return registryKey;
        }

        public static object? GetRegistryValue(string[] key, BuildInfo buildInfo) => GetRegistryKey(key.SkipLast(1), buildInfo).GetValue(key.Last());

        public static void SetRegistryValue<T>(string[] key, T value, BuildInfo buildInfo) where T : notnull => GetRegistryKey(key.SkipLast(1), buildInfo).SetValue(key.Last(), value);

        public static void DeleteRegistryValue(string[] key, BuildInfo buildInfo) => GetRegistryKey(key.SkipLast(1), buildInfo).DeleteValue(key.Last(), false);
    }
}
