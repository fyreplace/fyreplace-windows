using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.Views;
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
        [NotifyPropertyChangedFor(nameof(HasCurrentUser))]
        [NotifyCanExecuteChangedFor(nameof(UpdateAvatarCommand))]
        [NotifyCanExecuteChangedFor(nameof(LogoutCommand))]
        private User? currentUser;

        public string Username => CurrentUser?.Username ?? resources.GetString("Account_Username_Placeholder");

        public string DateJoined => CurrentUser != null
            ? string.Format(resources.GetString("Account_DateJoined"), CurrentUser.DateCreated.ToString("f"))
            : resources.GetString("Account_DateJoined_Placeholder");

        public bool HasCurrentUser => CurrentUser != null;

        private readonly IApiClient api = AppBase.GetService<IApiClient>();
        private readonly ResourceLoader resources = new();

        public AccountViewModel() => eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);

        [RelayCommand(CanExecute = nameof(HasCurrentUser))]
        public async Task UpdateAvatarAsync(Stream? stream)
        {
            if (stream == null)
            {
                var file = await AppBase.GetService<MainWindow>().PickImageFileAsync();

                if (file != null)
                {
                    using var s = await file.OpenStreamForReadAsync();
                    await UpdateAvatarAsync(s);
                }

                return;
            }

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

        [RelayCommand(CanExecute = nameof(HasCurrentUser))]
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
