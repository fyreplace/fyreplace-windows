using Fyreplace.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace Fyreplace.Pages
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage() => InitializeComponent();

        private bool AreInputsValid(string username, string email) => Users.IsUsernameValid(username) && Users.IsEmailValid(email);
    }
}
