using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;

namespace Fyreplace.ViewModels
{
    public sealed partial class AccountViewModel : ViewModelBase
    {
        private readonly IPreferences preferences = AppBase.GetService<IPreferences>();
        private readonly ISecrets secrets = AppBase.GetService<ISecrets>();

        [RelayCommand]
        public void Logout()
        {
            secrets.Token = string.Empty;
            preferences.Account_Identifier = string.Empty;
            preferences.Account_Username = string.Empty;
            preferences.Account_Email = string.Empty;
            preferences.Account_IsWaitingForRandomCode = false;
        }
    }
}
