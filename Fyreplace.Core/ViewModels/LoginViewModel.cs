using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Extensions;
using Fyreplace.Services;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class LoginViewModel(BuildInfo buildInfo, IPreferences preferences, ISecrets secrets, IEventBus eventBus, IStringsService stringsService, IApiClient api)
        : AccountEntryViewModelBase(buildInfo, preferences, secrets, eventBus, stringsService, api)
    {
        public override bool CanSubmitFirstStep => !string.IsNullOrWhiteSpace(preferences.Account_Identifier)
            && preferences.Account_Identifier.Length >= 3
            && preferences.Account_Identifier.Length <= 254;

        protected override async Task OnPreferenceChangedAsync(PreferenceChangedEvent e)
        {
            await base.OnPreferenceChangedAsync(e);

            switch (e.Name)
            {
                case nameof(IPreferences.Account_Identifier):
                    SubmitCommand.NotifyCanExecuteChanged();
                    break;
            }
        }

        protected override Task SendEmailAsync() => CallWhileLoadingAsync(async () =>
            {
                await api.CreateNewTokenAsync(true, new() { Identifier = preferences.Account_Identifier });
                preferences.Account_IsWaitingForRandomCode = true;
                IsRandomCodeTipShown = true;
            },
            onFailure: (statusCode, _, _) => statusCode switch
            {
                HttpStatusCode.BadRequest => new FailureEvent("Error_BadRequest"),
                HttpStatusCode.Forbidden => new FailureEvent("LoginPage_Error_Forbidden")
                    .Also(() => preferences.Account_IsWaitingForRandomCode = true),
                HttpStatusCode.NotFound => new FailureEvent("LoginPage_Error_NotFound"),
                _ => new FailureEvent()
            }
        );

        protected override Task CreateTokenAsync() => CallWhileLoadingAsync(async () =>
            {
                var token = await api.CreateTokenAsync(new()
                {
                    Identifier = preferences.Account_Identifier,
                    Secret = RandomCode
                });
                preferences.Account_Username = string.Empty;
                preferences.Account_Email = string.Empty;
                preferences.Account_IsWaitingForRandomCode = false;
                secrets.Token = token;
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
