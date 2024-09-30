using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace Fyreplace.Views.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private readonly string appVersion = AppBase.GetService<BuildInfo>().Version.Main;
        private readonly IEventBus eventBus = AppBase.GetService<IEventBus>();
        private readonly ISecrets secrets = AppBase.GetService<ISecrets>();
        private readonly AccountViewModel accountViewModel = AppBase.GetService<AccountViewModel>();

        public SettingsPage()
        {
            InitializeComponent();
            eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);
        }

        private Task OnSecretChangedAsync(SecretChangedEvent e)
        {
            switch (e.Name)
            {
                case nameof(ISecrets.Token):
                    if (string.IsNullOrEmpty(secrets.Token))
                    {
                        Profile.IsExpanded = false;
                    }

                    break;
            }

            return Task.CompletedTask;
        }
    }
}
