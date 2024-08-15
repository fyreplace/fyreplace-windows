using Fyreplace.Data.Preferences;
using System.Collections.Generic;

namespace Fyreplace.Tests.Data.Preferences
{
    public sealed class MemoryPreferences : PreferencesBase<string>
    {
        private readonly Dictionary<string, object> preferences = [];

        public override string MakeCleanKey(string key) => key;

        public override string Read(string key, string defaultValue)
        {
            if (preferences.TryGetValue(key, out object? value))
            {
                return value as string ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public override int Read(string key, int defaultValue)
        {
            if (preferences.TryGetValue(key, out object? value))
            {
                return value as int? ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public override bool Read(string key, bool defaultValue)
        {
            if (preferences.TryGetValue(key, out object? value))
            {
                return value as bool? ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public override void Write(string key, string value) => preferences[key] = value;

        public override void Write(string key, int value) => preferences[key] = value;

        public override void Write(string key, bool value) => preferences[key] = value;

        public void Clear() => preferences.Clear();
    }
}
