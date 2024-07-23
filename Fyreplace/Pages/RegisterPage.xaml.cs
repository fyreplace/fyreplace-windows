using Microsoft.UI.Xaml;
using System.Collections.Generic;
using static Fyreplace.Helpers.Users;

namespace Fyreplace.Pages
{
    public sealed partial class RegisterPage : AccountPageBase
    {
        protected override IDictionary<string, UIElement> ConnectedElements => new Dictionary<string, UIElement>()
        {
            ["title"] = Title,
            ["first-field"] = Username,
            ["submit"] = Submit
        };

        public RegisterPage() => InitializeComponent();

        private bool AreInputsValid(string username, string email) => IsUsernameValid(username) && IsEmailValid(email);
    }
}
