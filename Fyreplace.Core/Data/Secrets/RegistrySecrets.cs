using Fyreplace.Config;
using Fyreplace.Events;
using static Fyreplace.Data.RegistryUtils;

namespace Fyreplace.Data.Secrets
{
    public sealed partial class RegistrySecrets(BuildInfo buildInfo, IPreferences preferences, IEventBus eventBus) : SecretsBase<string[]>(preferences, eventBus)
    {
        public override string[] MakeCleanKey(string key) => key.Split('_');

        public override string Read(string[] key, string defaultValue) => GetRegistryValue(key, buildInfo) as string ?? defaultValue;

        public override void Write(string[] key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                DeleteRegistryValue(key, buildInfo);
            }
            else
            {
                SetRegistryValue(key, value, buildInfo);
            }
        }
    }
}
