using Fyreplace.Data;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Fyreplace.Views.Pages
{
    public abstract class RegisterPageBase : AccountEntryPageBase<RegisterViewModel>
    {
    }

    public sealed partial class RegisterPage : RegisterPageBase
    {
        protected override IDictionary<string, UIElement> ConnectedElements => new Dictionary<string, UIElement>
        {
            ["title"] = Title,
            ["first-field"] = Username
        };
        protected override RegisterViewModel viewModel => AppBase.GetService<RegisterViewModel>();

        private readonly IPreferences preferences = AppBase.GetService<IPreferences>();

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
            else if (preferences.Account_IsWaitingForRandomCode)
            {
                RandomCode.Focus(FocusState.Programmatic);
            }
        }
    }
}
