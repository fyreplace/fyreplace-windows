using Microsoft.Win32;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.Linq;

namespace Fyreplace.Data
{
    public static class RegistryUtils
    {
        public static RegistryKey GetRegistryKey(IEnumerable<string> path)
        {
            var resourceLoader = new ResourceLoader();
            var registryKey = Registry.CurrentUser
                .CreateSubKey("Software")
                .CreateSubKey(resourceLoader.GetString("AppName"));

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
