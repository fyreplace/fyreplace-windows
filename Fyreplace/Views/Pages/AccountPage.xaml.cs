using Fyreplace.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Fyreplace.Views.Pages
{
    public sealed partial class AccountPage : Page
    {
        private AccountViewModel viewModel => AppBase.GetService<AccountViewModel>();

        public AccountPage() => InitializeComponent();
    }
}
