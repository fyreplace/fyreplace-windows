using static Fyreplace.Data.RegistryUtils;

namespace Fyreplace.Data.Preferences
{
    public sealed class RegistryPreferences : PreferencesBase<string[]>
    {
        public override string[] MakeCleanKey(string key) => key.Split('_');

        public override string Read(string[] key, string defaultValue) => GetRegistryValue(key) as string ?? defaultValue;

        public override int Read(string[] key, int defaultValue) => GetRegistryValue(key) as int? ?? defaultValue;

        public override bool Read(string[] key, bool defaultValue) => Read(key, defaultValue ? 1 : 0) == 1;

        public override void Write(string[] key, string value) => SetRegistryValue(key, value);

        public override void Write(string[] key, int value) => SetRegistryValue(key, value);

        public override void Write(string[] key, bool value) => Write(key, value ? 1 : 0);
    }
}
