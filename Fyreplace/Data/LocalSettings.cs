using Windows.Storage;

namespace Fyreplace.Data
{
    public class LocalSettings : ISettings
    {
        public string Read(string key, string defaultValue) => ApplicationData.Current.LocalSettings.Values[key] as string ?? defaultValue;

        public void Write(string key, string value) => ApplicationData.Current.LocalSettings.Values[key] = value;
    }
}
