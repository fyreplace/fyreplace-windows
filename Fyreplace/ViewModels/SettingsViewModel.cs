using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fyreplace.Collections;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanAddEmail))]
        [NotifyCanExecuteChangedFor(nameof(AddEmailCommand))]
        public partial string NewEmail { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsRandomCodeTipShown { get; set; }

        public bool CanAddEmail => !string.IsNullOrWhiteSpace(NewEmail);
        public ObservableCollection<Email> Emails => emails;

        private readonly PagingCollection<Email> emails;

        public SettingsViewModel()
        {
            emails = new(api.ListEmailsAsync);
            eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);
            eventBus.Subscribe<EmailVerifiedEvent>(OnEmailVerifiedAsync);
        }

        [RelayCommand]
        public async Task LoadEmailsAsync()
        {
            emails.Reset();
            bool? hasMore;

            do
            {
                hasMore = await CallAsync(emails.FetchMoreAsync);
            }
            while (hasMore == true);

            foreach (var email in emails)
            {
                email.VerificationRequested += OnEmailVerificationRequestedAsync;
            }
        }

        [RelayCommand(CanExecute = nameof(CanAddEmail))]
        public async Task AddEmailAsync()
        {
            var email = await CallAsync(
                () => api.CreateEmailAsync(true, new EmailCreation { Email = NewEmail }),
                onFailure: (statusCode, _, _) => statusCode switch
                {
                    HttpStatusCode.BadRequest => new FailureEvent("SettingsPage_Error_Email_BadRequest"),
                    HttpStatusCode.Conflict => new FailureEvent("SettingsPage_Error_Email_Conflict"),
                    _ => new FailureEvent()
                }
            );

            if (email != null)
            {
                NewEmail = string.Empty;
                IsRandomCodeTipShown = true;
                emails.Add(email);
                email.VerificationRequested += OnEmailVerificationRequestedAsync;
            }
        }

        private Task OnEmailVerificationRequestedAsync(Email sender, EmailVerificationEventArgs e) => eventBus.PublishAsync(new EmailVerificationEvent(sender.Email1, e.Code));

        private async Task OnSecretChangedAsync(SecretChangedEvent e)
        {
            switch (e.Name)
            {
                case nameof(ISecrets.Token):
                    if (string.IsNullOrEmpty(secrets.Token))
                    {
                        emails.Reset();
                    }
                    else
                    {
                        await LoadEmailsAsync();
                    }

                    break;
            }
        }

        private Task OnEmailVerifiedAsync(EmailVerifiedEvent e)
        {
            var email = emails.First(email => email.Email1 == e.Email);

            if (email != null)
            {
                MakeVerified(email);
            }

            return Task.CompletedTask;
        }

        private void MakeVerified(Email email)
        {
            emails.Remove(email);
            email.Verified = true;
            emails.Add(email);
        }
    }
}
