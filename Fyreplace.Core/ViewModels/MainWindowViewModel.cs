using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        public partial bool ShowUserConnectionTip { get; set; }

        [ObservableProperty]
        public partial bool ShowEmailVerificationTip { get; set; }

        private readonly AccountEntryViewModelBase accountEntryViewModel;

        public MainWindowViewModel(
            IPreferences preferences,
            ISecrets secrets,
            IEventBus eventBus,
            IApiClient api,
            RegisterViewModel registerViewModel,
            LoginViewModel loginViewModel
        )
            : base(preferences, secrets, eventBus, api)
        {
            accountEntryViewModel = preferences.Account_IsRegistering ? registerViewModel : loginViewModel;
            eventBus.Subscribe<EmailVerificationEvent>(OnEmailVerificationAsync);
        }

        public async Task CompleteUserConnectionAsync(string randomCode)
        {
            if (!preferences.Account_IsWaitingForRandomCode)
            {
                return;
            }

            accountEntryViewModel.RandomCode = randomCode;
            ShowUserConnectionTip = true;
            var task = accountEntryViewModel.SubmitCommand.ExecuteAsync(null);
            var delay = Task.Delay(500);
            await Task.WhenAll(task, delay);
            ShowUserConnectionTip = false;
        }

        public async Task CompleteEmailVerificationAsync(string email, string code)
        {
            ShowEmailVerificationTip = true;
            var task = CallAsync(
                async () =>
                {
                    await api.VerifyEmailAsync(new EmailVerification { Email = email, Code = code });
                    await eventBus.PublishAsync(new EmailVerifiedEvent(email));
                },
                onFailure: (statusCode, _, _) => statusCode switch
                {
                    HttpStatusCode.NotFound => new FailureEvent("MainWindow_Error_Verification_NotFound"),
                    _ => new FailureEvent()
                }
            );
            var delay = Task.Delay(500);
            await Task.WhenAll(task, delay);
            ShowEmailVerificationTip = false;
        }

        private Task OnEmailVerificationAsync(EmailVerificationEvent e) => CompleteEmailVerificationAsync(e.Email, e.RandomCode);
    }
}
