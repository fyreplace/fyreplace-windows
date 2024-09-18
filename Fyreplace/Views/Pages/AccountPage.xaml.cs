using Fyreplace.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Fyreplace.Views.Pages
{
    public sealed partial class AccountPage : Page
    {
        private readonly AccountViewModel viewModel = AppBase.GetService<AccountViewModel>();

        public AccountPage() => InitializeComponent();
    }
}
