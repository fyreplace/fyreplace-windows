using Windows.Storage;

namespace Fyreplace.Data.Preferences
{
    public sealed class LocalSettingsPreferences : PreferencesBase<string>
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public override string MakeCleanKey(string key) => key.Replace('_', '.');

        public override string Read(string key, string defaultValue) => localSettings.Values[key] as string ?? defaultValue;

        public override int Read(string key, int defaultValue) => localSettings.Values[key] as int? ?? defaultValue;

        public override bool Read(string key, bool defaultValue) => localSettings.Values[key] as bool? ?? defaultValue;

        public override void Write(string key, string value) => localSettings.Values[key] = value;

        public override void Write(string key, int value) => localSettings.Values[key] = value;

        public override void Write(string key, bool value) => localSettings.Values[key] = value;
    }
}
