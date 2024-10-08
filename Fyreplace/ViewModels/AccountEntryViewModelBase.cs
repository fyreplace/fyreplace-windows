using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.ViewModels
{
    public abstract partial class AccountEntryViewModelBase : ViewModelBase
    {
        public abstract bool CanSubmitFirstStep { get; }

        public bool CanSubmitLastStep => RandomCode.Length >= 8;

        public bool CanSubmit => !IsLoading && (preferences.Account_IsWaitingForRandomCode ? CanSubmitLastStep : CanSubmitFirstStep);

        public bool CanCancel => !IsLoading;

        public int SelectedEnvironmentIndex
        {
            get => Array.IndexOf(environments, preferences.Connection_Environment);
            set
            {
                preferences.Connection_Environment = environments[value];
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> EnvironmentNames => environments.Select(e => e.Description());

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool isLoading = false;

        [ObservableProperty]
        private bool isRandomCodeTipShown = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string randomCode = string.Empty;

        protected static IApiClient Api => AppBase.GetService<IApiClient>();
        private static readonly Environment[] environments = Enum.GetValues<Environment>();

        public AccountEntryViewModelBase() => eventBus.Subscribe<PreferenceChangedEvent>(OnPreferenceChangedAsync);

        protected abstract Task CreateTokenAsync();

        protected abstract Task SendEmailAsync();

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        public Task SubmitAsync()
        {
            if (preferences.Account_IsWaitingForRandomCode)
            {
                return CreateTokenAsync();
            }
            else
            {
                return SendEmailAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanCancel))]
        public void Cancel()
        {
            preferences.Account_IsWaitingForRandomCode = false;
            preferences.Account_IsRegistering = false;
            IsRandomCodeTipShown = false;
        }

        protected Task CallWhileLoadingAsync(Func<Task> action, Func<HttpStatusCode, ViolationReport?, ExplainedFailure?, FailureEvent?> onFailure) => CallAsync(async () =>
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

        protected virtual Task OnPreferenceChangedAsync(PreferenceChangedEvent e)
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
