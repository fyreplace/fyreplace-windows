using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Fyreplace.Views.Pages
{
    public abstract class LoginPageBase : AccountEntryPageBase<LoginViewModel>
    {
    }

    public sealed partial class LoginPage : LoginPageBase
    {
        protected override IDictionary<string, UIElement> ConnectedElements => new Dictionary<string, UIElement>
        {
            ["title"] = Title,
            ["first-field"] = Identifier,
            ["submit"] = Submit,
            ["submit-wrapper"] = SubmitWrapper
        };
        protected override LoginViewModel viewModel => AppBase.GetService<LoginViewModel>();

        private readonly IPreferences preferences = AppBase.GetService<IPreferences>();

        public LoginPage() => InitializeComponent();

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            if (!viewModel.CanSubmitFirstStep)
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
