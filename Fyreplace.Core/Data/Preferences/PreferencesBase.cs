using Fyreplace.Config;
using Fyreplace.Events;
using System;

namespace Fyreplace.Data.Preferences
{
    public abstract class PreferencesBase<K> : DataStoreBase<K>, IPreferences
    {
        private readonly BuildInfo buildInfo;

        public PreferencesBase(BuildInfo buildInfo, IEventBus eventBus)
        {
            this.buildInfo = buildInfo;
            PropertyChanged += (sender, args) => eventBus.PublishAsync(new PreferenceChangedEvent(args.PropertyName!));
        }

        public abstract string Read(K key, string defaultValue);

        public abstract int Read(K key, int defaultValue);

        public abstract bool Read(K key, bool defaultValue);

        public abstract void Write(K key, string value);

        public abstract void Write(K key, int value);

        public abstract void Write(K key, bool value);

        #region Connection

        public Environment Connection_Environment
        {
            get
            {
                try
                {
                    return Enum.Parse<Environment>(Read(MakeKey(), defaultValue: EnvironmentExtensions.Default(buildInfo).ToString())
);
                }
                catch
                {
                    return EnvironmentExtensions.Default(buildInfo);
                }
            }

            set
            {
                Write(MakeKey(), value.ToString());
                OnPropertyChanged();
            }
        }

        #endregion

        #region Account

        public string Account_Identifier
        {
            get => Read(MakeKey(), defaultValue: string.Empty);
            set
            {
                Write(MakeKey(), value);
                OnPropertyChanged();
            }
        }

        public string Account_Username
        {
            get => Read(MakeKey(), defaultValue: string.Empty);
            set
            {
                Write(MakeKey(), value);
                OnPropertyChanged();
            }
        }

        public string Account_Email
        {
            get => Read(MakeKey(), defaultValue: string.Empty);
            set
            {
                Write(MakeKey(), value);
                OnPropertyChanged();
            }
        }

        public bool Account_IsWaitingForRandomCode
        {
            get => Read(MakeKey(), defaultValue: false);
            set
            {
                Write(MakeKey(), value);
                OnPropertyChanged();
            }
        }

        public bool Account_IsRegistering
        {
            get => Read(MakeKey(), defaultValue: false);
            set
            {
                Write(MakeKey(), value);
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
