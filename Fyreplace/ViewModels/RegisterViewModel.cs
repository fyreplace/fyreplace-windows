using Fyreplace.Data;
using Fyreplace.Events;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class RegisterViewModel : AccountEntryViewModelBase
    {
        public override bool CanSubmitFirstStep => IsUsernameValid && IsEmailValid;

        public bool IsUsernameValid => !string.IsNullOrWhiteSpace(preferences.Account_Username)
            && preferences.Account_Username.Length >= 3
            && preferences.Account_Username.Length <= 50;

        public bool IsEmailValid => preferences.Account_Email.Contains('@')
            && preferences.Account_Email.Length >= 3
            && preferences.Account_Email.Length <= 254;

        public override Task Submit() => Task.CompletedTask;

        protected override async Task OnPreferenceChanged(PreferenceChangedEvent e)
        {
            await base.OnPreferenceChanged(e);

            switch (e.Name)
            {
                case nameof(IPreferences.Account_Username):
                case nameof(IPreferences.Account_Email):
                    SubmitCommand.NotifyCanExecuteChanged();
                    break;
            }
        }
    }
}
