using Fyreplace.Events;

namespace Fyreplace.Data.Secrets
{
    public abstract class SecretsBase<K> : DataStoreBase<K>, ISecrets
    {
        private readonly IPreferences preferences;

        public SecretsBase(IPreferences preferences, IEventBus eventBus)
        {
            this.preferences = preferences;
            PropertyChanged += (sender, args) => eventBus.PublishAsync(new SecretChangedEvent(args.PropertyName!));
        }

        public string Token
        {
            get => Read(MakeCleanKey(GetTokenKey()), string.Empty);

            set
            {
                Write(MakeCleanKey(GetTokenKey()), value);
                OnPropertyChanged();
            }
        }

        public abstract string Read(K key, string defaultValue);

        public abstract void Write(K key, string value);

        private string GetTokenKey()
        {
            var identifier = preferences.Account_Identifier;
            return identifier.Length > 0 ? identifier : preferences.Account_Username;
        }
    }
}
