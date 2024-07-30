using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using System.Collections.Generic;

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

        private readonly RegisterViewModel viewModel = AppBase.GetService<RegisterViewModel>();

        public RegisterPage() => InitializeComponent();

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            if (!viewModel.IsUsernameValid)
            {
                Username.Focus(FocusState.Programmatic);
            }
            else if (!viewModel.IsEmailValid)
            {
                Email.Focus(FocusState.Programmatic);
            }
        }
    }
}
