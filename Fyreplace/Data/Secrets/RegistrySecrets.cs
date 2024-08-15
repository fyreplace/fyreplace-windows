using static Fyreplace.Data.RegistryUtils;

namespace Fyreplace.Data.Secrets
{
    public sealed class RegistrySecrets : SecretsBase<string[]>
    {
        public override string[] MakeCleanKey(string key) => key.Split('_');

        public override string Read(string[] key, string defaultValue) => GetRegistryValue(key) as string ?? defaultValue;

        public override void Write(string[] key, string value)
        {
            if (value == string.Empty)
            {
                DeleteRegistryValue(key);
            }
            else
            {
                SetRegistryValue(key, value);
            }
        }
    }
}
