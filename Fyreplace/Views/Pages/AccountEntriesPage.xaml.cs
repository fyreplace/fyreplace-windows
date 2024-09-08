using Fyreplace.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace Fyreplace.Views.Pages
{
    public sealed partial class AccountEntriesPage : Page
    {
        private readonly IPreferences preferences = AppBase.GetService<IPreferences>();

        public AccountEntriesPage()
        {
            InitializeComponent();
            SelectorBar.SelectedItem = preferences.Account_IsRegistering ? Register : Login;
        }

        private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            Host.Navigate(
                sender.SelectedItem == Login ? typeof(LoginPage) : typeof(RegisterPage),
                null,
                new SuppressNavigationTransitionInfo()
            );
            Host.BackStack.Clear();
        }
    }
}
