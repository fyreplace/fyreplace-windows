using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Fyreplace.Pages
{
    public sealed partial class LoginPage : AccountPageBase
    {
        private readonly LoginViewModel viewModel = AppBase.GetService<LoginViewModel>();

        protected override IDictionary<string, UIElement> ConnectedElements => new Dictionary<string, UIElement>()
        {
            ["title"] = Title,
            ["first-field"] = Identifier,
            ["submit"] = Submit
        };

        public LoginPage() => InitializeComponent();
    }
}
