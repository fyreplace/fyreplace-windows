using Fyreplace.Config;
using System;
using Windows.Security.Credentials;

namespace Fyreplace.Data.Secrets
{
    public sealed class PasswordVaultSecrets : SecretsBase<string>
    {
        private readonly PasswordVault vault = new();
        private readonly BuildInfo buildInfo = AppBase.GetService<BuildInfo>();
        private readonly IPreferences preferences = AppBase.GetService<IPreferences>();
        private string Resource => buildInfo.Api.ForEnvironment(preferences.Connection_Environment).ToString();

        public override string MakeCleanKey(string key) => key;

        public override string Read(string key, string defaultValue)
        {
            try
            {
                var credential = vault.Retrieve(Resource, key);
                credential.RetrievePassword();
                return credential.Password;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public override void Write(string key, string value)
        {
            if (value == string.Empty)
            {
                Delete(key);
                return;
            }

            try
            {
                var credential = vault.Retrieve(Resource, key);
                credential.Password = value;
                vault.Add(credential);
            }
            catch (Exception)
            {
                vault.Add(new PasswordCredential(Resource, key, value));
            }
        }

        private void Delete(string key)
        {
            try
            {
                vault.Remove(vault.Retrieve(Resource, key));
            }
            catch (Exception)
            {
            }
        }
    }
}
