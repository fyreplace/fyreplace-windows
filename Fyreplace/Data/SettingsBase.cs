using System;

namespace Fyreplace.Data
{
    public interface ISettings
    {
        public Environment Environment
        {
            get
            {
                try
                {
                    return (Environment)Enum.Parse(
                        typeof(Environment),
                        Read("Environment", EnvironmentExtensions.Default.ToString())
                    );
                }
                catch
                {
                    return EnvironmentExtensions.Default;
                }
            }

            set => Write("Environment", value.ToString());
        }

        public string Read(string key, string defaultValue);

        public void Write(string key, string value);
    }
}
