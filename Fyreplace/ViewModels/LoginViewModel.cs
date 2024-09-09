using Fyreplace.Data;
using Fyreplace.Events;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class LoginViewModel : AccountEntryViewModelBase
    {
        public override bool CanSubmitFirstStep => !string.IsNullOrWhiteSpace(preferences.Account_Identifier)
            && preferences.Account_Identifier.Length >= 3
            && preferences.Account_Identifier.Length <= 254;

        protected override async Task OnPreferenceChanged(PreferenceChangedEvent e)
        {
            await base.OnPreferenceChanged(e);

            switch (e.Name)
            {
                case nameof(IPreferences.Account_Identifier):
                    SubmitCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        protected override Task SendEmail() => CallWhileLoading(async () =>
            {
                await Api.CreateNewTokenAsync(true, new() { Identifier = preferences.Account_Identifier });
                preferences.Account_IsWaitingForRandomCode = true;
                IsRandomCodeTipShown = true;
            },
            onFailure: (statusCode, _, _) => statusCode switch
            {
                HttpStatusCode.BadRequest => new FailureEvent("Error_BadRequest"),
                HttpStatusCode.NotFound => new FailureEvent("LoginPage_Error_NotFound"),
                _ => new FailureEvent()
            }
        );

        protected override Task CreateToken() => CallWhileLoading(async () =>
            {
                secrets.Token = await Api.CreateTokenAsync(new()
                {
                    Identifier = preferences.Account_Identifier,
                    Secret = RandomCode
                });
                preferences.Account_IsWaitingForRandomCode = false;
            },
            onFailure: (statusCode, _, _) => statusCode switch
            {
                HttpStatusCode.BadRequest => new FailureEvent("AccountEntryPageBase_Error_CreateToken_BadRequest"),
                HttpStatusCode.NotFound => new FailureEvent("LoginPage_Error_NotFound"),
                _ => new FailureEvent()
            }
        );
    }
}
