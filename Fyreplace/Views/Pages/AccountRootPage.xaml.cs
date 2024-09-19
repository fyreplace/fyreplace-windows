using Fyreplace.Data;
using Fyreplace.Events;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Threading.Tasks;

namespace Fyreplace.Views.Pages
{
    public sealed partial class AccountRootPage : Page
    {
        private readonly ISecrets secrets = AppBase.GetService<ISecrets>();
        private readonly IEventBus eventBus = AppBase.GetService<IEventBus>();

        public AccountRootPage()
        {
            InitializeComponent();
            UpdateContent();
            eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);
        }

        private void UpdateContent()
        {
            Host.Navigate(
                string.IsNullOrEmpty(secrets.Token)
                    ? typeof(AccountEntriesPage)
                    : typeof(AccountPage),
                null,
                new SuppressNavigationTransitionInfo()
            );
            Host.BackStack.Clear();
        }

        private Task OnSecretChangedAsync(SecretChangedEvent e)
        {
            switch (e.Name)
            {
                case nameof(ISecrets.Token):
                    UpdateContent();
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
