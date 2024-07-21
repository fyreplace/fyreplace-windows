using Fyreplace.Data;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.Pages
{
    public abstract class AccountPageBase : Page
    {
        protected Environment[] Environments
        {
            get
            {
                Environment[] value = [
                    Environment.Main,
                    Environment.Dev
                ];

#if DEBUG
                return [.. value, Environment.Local];
#else
                return value;
#endif
            }
        }

        protected IEnumerable<string> EnvironmentNames => Environments.Select(e => e.Description());

        protected int SelectedEnvironmentIndex
        {
            get => Array.IndexOf(Environments, settings.Environment);
            set => settings.Environment = Environments[value];
        }

        private readonly ISettings settings = AppBase.GetService<ISettings>();
    }
}
