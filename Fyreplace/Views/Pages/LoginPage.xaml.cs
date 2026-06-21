using Fyreplace.Data;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;

namespace Fyreplace.Views.Pages
{
    public abstract class LoginPageBase : AccountEntryPageBase<LoginViewModel>
    {
    }

    public sealed partial class LoginPage : LoginPageBase
    {
        protected override LoginViewModel ViewModel => App.GetService<LoginViewModel>();

        private readonly IPreferences preferences = App.GetService<IPreferences>();

        public LoginPage() => InitializeComponent();

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.CanSubmitFirstStep)
            {
                Identifier.Focus(FocusState.Programmatic);
            }
            else if (preferences.Account_IsWaitingForRandomCode)
            {
                RandomCode.Focus(FocusState.Programmatic);
            }
        }
    }
}
