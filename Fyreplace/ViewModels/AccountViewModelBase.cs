using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.ViewModels
{
    public abstract partial class AccountViewModelBase : ViewModelBase
    {
        public abstract bool CanSubmit { get; }

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

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public bool isLoading = false;

        private int selectedEnvironmentIndex;
        private readonly ISettings settings = AppBase.GetService<ISettings>();

        private static readonly Environment[] Environments = Enum.GetValues<Environment>();

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        public abstract Task Submit();

        protected Task CallWhileLoading(Func<Task> action) => Call(async () =>
        {
            try
            {
                IsLoading = true;
                await action();
            }
            finally
            {
                IsLoading = false;
            }
        });
    }
}
