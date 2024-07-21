using Microsoft.Win32;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Fyreplace.Data
{
    public class RegistrySettings : ISettings
    {
        public string Read(string key, string defaultValue) => GetRegistryRootKey().GetValue(key) as string ?? defaultValue;

        public void Write(string key, string value) => GetRegistryRootKey().SetValue(key, value);

        private static RegistryKey GetRegistryRootKey()
        {
            var resourceLoader = new ResourceLoader();
            return Registry.CurrentUser
                .CreateSubKey("Software")
                .CreateSubKey(resourceLoader.GetString("AppName"));
        }
    }
}
