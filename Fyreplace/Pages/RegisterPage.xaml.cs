using static Fyreplace.Helpers.Users;

namespace Fyreplace.Pages
{
    public sealed partial class RegisterPage : AccountPageBase
    {
        public RegisterPage() => InitializeComponent();

        private bool AreInputsValid(string username, string email) => IsUsernameValid(username) && IsEmailValid(email);
    }
}
