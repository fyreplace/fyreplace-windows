using Fyreplace.Data;
using System.Collections.Generic;

namespace Fyreplace.Tests.Data
{
    public class MemorySettings : ISettings
    {
        private readonly IDictionary<string, string> settings = new Dictionary<string, string>();

        public string Read(string key, string defaultValue) => settings[key] ?? defaultValue;

        public void Write(string key, string value) => settings[key] = value;
    }
}
