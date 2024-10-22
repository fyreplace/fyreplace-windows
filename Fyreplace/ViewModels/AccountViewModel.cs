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
        [NotifyPropertyChangedFor(nameof(Bio))]
        [NotifyPropertyChangedFor(nameof(HasCurrentUser))]
        [NotifyPropertyChangedFor(nameof(CanUpdateAvatar))]
        [NotifyPropertyChangedFor(nameof(CanRemoveAvatar))]
        [NotifyCanExecuteChangedFor(nameof(UpdateAvatarCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveAvatarCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateBioCommand))]
        [NotifyCanExecuteChangedFor(nameof(LogoutCommand))]
        private User? currentUser;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanUpdateAvatar))]
        [NotifyPropertyChangedFor(nameof(CanRemoveAvatar))]
        [NotifyCanExecuteChangedFor(nameof(UpdateAvatarCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveAvatarCommand))]
        private bool isLoadingAvatar;

        public string Username => CurrentUser?.Username ?? resources.GetString("Account_Username_Placeholder");
        public string DateJoined => CurrentUser != null
            ? string.Format(resources.GetString("Account_DateJoined"), CurrentUser.DateCreated.ToString("f"))
            : resources.GetString("Account_DateJoined_Placeholder");
        public string Bio => !string.IsNullOrEmpty(CurrentUser?.Bio)
            ? CurrentUser?.Bio ?? string.Empty
            : resources.GetString("Account_Bio_Placeholder");
        public bool HasCurrentUser => CurrentUser != null;
        public bool CanUpdateAvatar => HasCurrentUser && !IsLoadingAvatar;
        public bool CanRemoveAvatar => !string.IsNullOrEmpty(CurrentUser?.Avatar) && !IsLoadingAvatar;

        private readonly IApiClient api = AppBase.GetService<IApiClient>();
        private readonly ResourceLoader resources = new();

        public AccountViewModel() => eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);

        [RelayCommand(CanExecute = nameof(CanUpdateAvatar))]
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
                async () =>
                {
                    try
                    {
                        IsLoadingAvatar = true;
                        return await api.SetCurrentUserAvatarAsync(stream);
                    }
                    finally
                    {
                        IsLoadingAvatar = false;
                    }
                },
                onFailure: (status, _, _) => status switch
                {
                    HttpStatusCode.RequestEntityTooLarge => new FailureEvent("Account_Error_RequestEntityTooLarge"),
                    HttpStatusCode.UnsupportedMediaType => new FailureEvent("Account_Error_UnsupportedMediaType"),
                    _ => new FailureEvent()
                }
            );

            if (avatar != null)
            {
                await SetCurrentUserAvatarAsync(avatar);
            }
        }

        [RelayCommand(CanExecute = nameof(CanRemoveAvatar))]
        public async Task RemoveAvatarAsync()
        {
            await CallAsync(api.DeleteCurrentUserAvatarAsync);
            await SetCurrentUserAvatarAsync(null);
        }

        [RelayCommand(CanExecute = nameof(HasCurrentUser))]
        public async Task UpdateBioAsync(string bio)
        {
            bio = await CallAsync(() => api.SetCurrentUserBioAsync(bio)) ?? string.Empty;

            if (CurrentUser != null)
            {
                CurrentUser.Bio = bio;
                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(Bio));
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

        private async Task SetCurrentUserAvatarAsync(string? avatar)
        {
            if (CurrentUser != null)
            {
                CurrentUser.Avatar = avatar ?? string.Empty;
                OnPropertyChanged(nameof(CanRemoveAvatar));
                RemoveAvatarCommand.NotifyCanExecuteChanged();
                await eventBus.PublishAsync(new ModelChangedEvent(CurrentUser.Id, nameof(User.Avatar)));
            }
        }
    }
}
