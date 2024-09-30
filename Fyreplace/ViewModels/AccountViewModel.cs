using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Microsoft.Windows.ApplicationModel.Resources;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class AccountViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Username))]
        [NotifyPropertyChangedFor(nameof(DateJoined))]
        private User? currentUser;

        public string Username => CurrentUser?.Username ?? resources.GetString("Loading");

        public string DateJoined => string.Format(resources.GetString("AccountPage_DateJoined"), CurrentUser?.DateCreated.ToString("g") ?? resources.GetString("Loading"));

        private readonly IApiClient api = AppBase.GetService<IApiClient>();
        private readonly ResourceLoader resources = new();

        public AccountViewModel() => eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);

        [RelayCommand]
        public async Task UpdateAvatarAsync(Stream stream)
        {
            var avatar = await CallAsync(
                () => api.SetCurrentUserAvatarAsync(stream),
                onFailure: (status, _, _) => status switch
                {
                    HttpStatusCode.RequestEntityTooLarge => new FailureEvent("Account_Error_RequestEntityTooLarge"),
                    HttpStatusCode.UnsupportedMediaType => new FailureEvent("Account_Error_UnsupportedMediaType"),
                    _ => new FailureEvent()
                }
            );

            if (CurrentUser != null && avatar != null)
            {
                CurrentUser.Avatar = avatar;
                await eventBus.PublishAsync(new ModelChangedEvent(CurrentUser.Id, nameof(User.Avatar)));
            }
        }

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
                    CurrentUser = string.IsNullOrEmpty(secrets.Token) ? null : await CallAsync(api.GetCurrentUserAsync);
                    break;
            }
        }
    }
}
