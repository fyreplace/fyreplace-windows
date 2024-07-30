using CommunityToolkit.Mvvm.ComponentModel;

namespace Fyreplace.ViewModels
{
    public sealed partial class RegisterViewModel : AccountViewModelBase
    {
        [ObservableProperty]
        public string username = "";

        [ObservableProperty]
        public string email = "";

        public bool IsUsernameValid => !string.IsNullOrWhiteSpace(Username)
            && Username.Length >= 3
            && Username.Length <= 50;

        public bool IsEmailValid => Email.Contains('@')
            && Email.Length >= 3
            && Email.Length <= 254;

        public bool CanSubmit => IsUsernameValid && IsEmailValid;
    }
}
