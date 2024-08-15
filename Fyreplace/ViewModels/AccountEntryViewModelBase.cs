using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.ViewModels
{
    public abstract partial class AccountEntryViewModelBase : ViewModelBase
    {
        public abstract bool CanSubmitFirstStep { get; }

        public bool CanSubmitLastStep => RandomCode.Length == 6;

        public bool CanSubmit => !IsLoading && (preferences.Account_IsWaitingForRandomCode ? CanSubmitLastStep : CanSubmitFirstStep);

        public bool CanCancel => !IsLoading;

        public int SelectedEnvironmentIndex
        {
            get => Array.IndexOf(Environments, preferences.Connection_Environment);
            set
            {
                preferences.Connection_Environment = Environments[value];
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> EnvironmentNames => Environments.Select(e => e.Description());

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool isLoading = false;

        [ObservableProperty]
        private bool isRandomCodeTipShown = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string randomCode = string.Empty;

        protected readonly IPreferences preferences = AppBase.GetService<IPreferences>();
        protected readonly ISecrets secrets = AppBase.GetService<ISecrets>();
        protected readonly IEventBus eventBus = AppBase.GetService<IEventBus>();

        private static readonly Environment[] Environments = Enum.GetValues<Environment>();

        public AccountEntryViewModelBase() => eventBus.Subscribe<PreferenceChangedEvent>(OnPreferenceChanged);

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        public abstract Task Submit();

        [RelayCommand(CanExecute = nameof(CanCancel))]
        public virtual void Cancel() => preferences.Account_IsWaitingForRandomCode = false;

        protected Task CallWhileLoading(Func<Task> action, Func<ApiException, FailureEvent?> onFailure) => Call(async () =>
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
            },
            onFailure
        );

        protected virtual Task OnPreferenceChanged(PreferenceChangedEvent e)
        {
            switch (e.Name)
            {
                case nameof(IPreferences.Account_IsWaitingForRandomCode):
                    SubmitCommand.NotifyCanExecuteChanged();
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
