using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Data;
using Fyreplace.Events;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class RegisterViewModel : AccountEntryViewModelBase
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private bool hasAcceptedTerms;

        public override bool CanSubmitFirstStep => IsUsernameValid && IsEmailValid && HasAcceptedTerms;

        public bool IsUsernameValid => !string.IsNullOrWhiteSpace(preferences.Account_Username)
            && preferences.Account_Username.Length >= 3
            && preferences.Account_Username.Length <= 50;

        public bool IsEmailValid => preferences.Account_Email.Contains('@')
            && preferences.Account_Email.Length >= 3
            && preferences.Account_Email.Length <= 254;

        public RegisterViewModel() => HasAcceptedTerms = preferences.Account_IsWaitingForRandomCode;

        protected override async Task OnPreferenceChangedAsync(PreferenceChangedEvent e)
        {
            await base.OnPreferenceChangedAsync(e);

            switch (e.Name)
            {
                case nameof(IPreferences.Account_Username):
                case nameof(IPreferences.Account_Email):
                    SubmitCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        protected override Task SendEmailAsync() => CallWhileLoading(async () =>
            {
                await Api.CreateUserAsync(true, new()
                {
                    Username = preferences.Account_Username,
                    Email = preferences.Account_Email
                });
                preferences.Account_IsWaitingForRandomCode = true;
                preferences.Account_IsRegistering = true;
                IsRandomCodeTipShown = true;
            },
            onFailure: (statusCode, violationReport, explainedFailure) => statusCode switch
            {
                HttpStatusCode.BadRequest => violationReport?.Violations?.FirstOrDefault()?.Field switch
                {
                    "createUser.input.username" => new FailureEvent("RegisterPage_Error_CreateUser_BadRequest_Username"),
                    "createUser.input.email" => new FailureEvent("RegisterPage_Error_CreateUser_BadRequest_Email"),
                    _ => new FailureEvent("Error_BadRequest")
                },
                HttpStatusCode.Forbidden => new FailureEvent("RegisterPage_Error_CreateUser_Forbidden"),
                HttpStatusCode.Conflict => new FailureEvent(
                    $"RegisterPage_Error_CreateUser_Conflict_{(explainedFailure?.Reason == "username_taken" ? "Username" : "Email")}"
                ),
                _ => new FailureEvent()
            });

        protected override Task CreateTokenAsync() => CallWhileLoading(async () =>
            {
                var  token = await Api.CreateTokenAsync(new()
                {
                    Identifier = preferences.Account_Email,
                    Secret = RandomCode
                });
                preferences.Account_Identifier = string.Empty;
                preferences.Account_IsWaitingForRandomCode = false;
                preferences.Account_IsRegistering = false;
                secrets.Token = token;
            },
            onFailure: (statusCode, _, _) => statusCode switch
            {
                HttpStatusCode.BadRequest => new FailureEvent("AccountEntryPageBase_Error_CreateToken_BadRequest"),
                HttpStatusCode.NotFound => new FailureEvent("RegisterPage_Error_CreateToken_NotFound"),
                _ => new FailureEvent()
            }
        );
    }
}
