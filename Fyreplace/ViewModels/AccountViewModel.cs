using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class AccountViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Username))]
        [NotifyPropertyChangedFor(nameof(DateJoined))]
        private User? user;

        public string Username => User?.Username ?? resources.GetString("Loading");

        public string DateJoined => string.Format(resources.GetString("AccountPage_DateJoined"), User?.DateCreated.ToString("g") ?? resources.GetString("Loading"));

        private readonly IApiClient api = AppBase.GetService<IApiClient>();
        private readonly ResourceLoader resources = new();

        public AccountViewModel() => eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);

        [RelayCommand]
        public void Logout()
        {
            secrets.Token = string.Empty;
            preferences.Account_Username = string.Empty;
            preferences.Account_Email = string.Empty;
        }

        private async Task OnSecretChangedAsync(SecretChangedEvent e)
        {
            switch (e.Name)
            {
                case nameof(ISecrets.Token):
                    User = secrets.Token != string.Empty ? await CallAsync(api.GetCurrentUserAsync) : null;
                    break;
            }
        }
    }
}
