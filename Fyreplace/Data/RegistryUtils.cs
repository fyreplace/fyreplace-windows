using Fyreplace.Config;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fyreplace.Data
{
    public static class RegistryUtils
    {
        public static RegistryKey GetRegistryKey(IEnumerable<string> path)
        {
            var registryKey = Registry.CurrentUser
                .CreateSubKey("Software")
                .CreateSubKey(AppBase.GetService<BuildInfo>().App.Name);

            foreach (var segment in path)
            {
                registryKey = registryKey.CreateSubKey(segment);
            }

            return registryKey;
        }

        public static object? GetRegistryValue(string[] key) => GetRegistryKey(key.SkipLast(1)).GetValue(key.Last());

        public static void SetRegistryValue<T>(string[] key, T value) where T : notnull => GetRegistryKey(key.SkipLast(1)).SetValue(key.Last(), value);

        public static void DeleteRegistryValue(string[] key) => GetRegistryKey(key.SkipLast(1)).DeleteValue(key.Last(), false);
    }
}
