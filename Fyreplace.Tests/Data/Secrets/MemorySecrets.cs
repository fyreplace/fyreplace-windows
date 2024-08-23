using Fyreplace.Data.Secrets;
using System.Collections.Generic;

namespace Fyreplace.Tests.Data.Secrets
{
    public sealed class MemorySecrets : SecretsBase<string>
    {
        private readonly Dictionary<string, string> secrets = [];

        public override string MakeCleanKey(string key) => key;

        public override string Read(string key, string defaultValue)
        {
            if (secrets.TryGetValue(key, out string? value))
            {
                return value ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public override void Write(string key, string value)
        {
            if (value == string.Empty)
            {
                secrets.Remove(key);
            }
            else
            {
                secrets[key] = value;
            }
        }
    }
}
