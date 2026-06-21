using Fyreplace.Data;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;

namespace Fyreplace.Views.Pages
{
    public abstract class RegisterPageBase : AccountEntryPageBase<RegisterViewModel>
    {
    }

    public sealed partial class RegisterPage : RegisterPageBase
    {
        protected override RegisterViewModel ViewModel => App.GetService<RegisterViewModel>();

        private readonly IPreferences preferences = App.GetService<IPreferences>();

        public RegisterPage() => InitializeComponent();

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsUsernameValid)
            {
                Username.Focus(FocusState.Programmatic);
            }
            else if (!ViewModel.IsEmailValid)
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
