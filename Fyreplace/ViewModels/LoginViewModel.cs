using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class LoginViewModel : AccountEntryViewModelBase
    {
        public override bool CanSubmitFirstStep => !string.IsNullOrWhiteSpace(preferences.Account_Identifier)
            && preferences.Account_Identifier.Length >= 3
            && preferences.Account_Identifier.Length <= 254;

        private static IApiClient Api => AppBase.GetService<IApiClient>();

        public override Task Submit() => CallWhileLoading(async () =>
        {
            if (preferences.Account_IsWaitingForRandomCode)
            {
                await CreateToken();
            }
            else
            {
                await SendEmail();
            }
        }, onFailure: (ApiException exception) =>
        {
            return exception.StatusCode switch
            {
                (int)HttpStatusCode.BadRequest => new FailureEvent("LoginPage_Error_BadRequest"),
                (int)HttpStatusCode.NotFound => new FailureEvent("LoginPage_Error_NotFound"),
                _ => new FailureEvent()
            };
        });

        public override void Cancel()
        {
            base.Cancel();
            IsRandomCodeTipShown = false;
        }

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

        private Task SendEmail() => CallWhileLoading(async () =>
            {
                await Api.CreateNewTokenAsync(new() { Identifier = preferences.Account_Identifier });
                preferences.Account_IsWaitingForRandomCode = true;
                IsRandomCodeTipShown = true;
            },
            onFailure: (ApiException exception) => exception.StatusCode switch
            {
                (int)HttpStatusCode.BadRequest => new FailureEvent("Error_BadRequest"),
                (int)HttpStatusCode.NotFound => new FailureEvent("LoginPage_Error_NotFound"),
                _ => new FailureEvent()
            }
        );

        private Task CreateToken() => CallWhileLoading(async () =>
            {
                secrets.Token = await Api.CreateTokenAsync(new()
                {
                    Identifier = preferences.Account_Identifier,
                    Secret = RandomCode
                });
                preferences.Account_Identifier = string.Empty;
                preferences.Account_IsWaitingForRandomCode = false;
            },
            onFailure: (ApiException exception) => exception.StatusCode switch
            {
                (int)HttpStatusCode.BadRequest => new FailureEvent("LoginPage_Error_CreateToken_BadRequest"),
                (int)HttpStatusCode.NotFound => new FailureEvent("LoginPage_Error_NotFound"),
                _ => new FailureEvent()
            }
        );
    }
}
