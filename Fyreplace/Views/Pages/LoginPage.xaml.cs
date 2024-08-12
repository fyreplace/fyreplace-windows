using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Fyreplace.Views.Pages
{
    public sealed partial class LoginPage : AccountPageBase
    {
        private readonly LoginViewModel viewModel = AppBase.GetService<LoginViewModel>();

        protected override IDictionary<string, UIElement> ConnectedElements => new Dictionary<string, UIElement>
        {
            ["title"] = Title,
            ["first-field"] = Identifier,
            ["submit"] = Submit,
            ["submit-wrapper"] = SubmitWrapper
        };

        public LoginPage() => InitializeComponent();

        #region Event Handlers

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            if (!viewModel.CanSubmit)
            {
                Identifier.Focus(FocusState.Programmatic);
            }
        }

        #endregion
    }
}
