using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.ViewModels
{
    public abstract partial class AccountViewModelBase : ObservableObject
    {
        public IEnumerable<string> EnvironmentNames => Environments.Select(e => e.Description());

        public int SelectedEnvironmentIndex
        {
            get => Array.IndexOf(Environments, settings.Environment);
            set
            {
                settings.Environment = Environments[value];
                selectedEnvironmentIndex = value;
                SetProperty(ref selectedEnvironmentIndex, value);
            }
        }

        private int selectedEnvironmentIndex;
        private readonly ISettings settings = AppBase.GetService<ISettings>();

        private static Environment[] Environments
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
    }
}
