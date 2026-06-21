using CommunityToolkit.Mvvm.Input;
using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Fyreplace.Views.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private readonly string appVersion = App.GetService<BuildInfo>().Version.Main;
        private readonly IEventBus eventBus = App.GetService<IEventBus>();
        private readonly ISecrets secrets = App.GetService<ISecrets>();
        private readonly IStringsService stringsService = App.GetService<IStringsService>();
        private readonly AccountViewModel accountViewModel = App.GetService<AccountViewModel>();
        private readonly SettingsViewModel viewModel = App.GetService<SettingsViewModel>();

        public SettingsPage()
        {
            InitializeComponent();
            eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);
        }

        [RelayCommand]
        private async Task EditBioAsync()
        {
            var textBox = new TextBox
            {
                MaxHeight = 200,
                MaxLength = 3000,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Text = accountViewModel.CurrentUser?.Bio
            };
            var dialog = new ContentDialog
            {
                XamlRoot = Content.XamlRoot,
                Title = stringsService.GetString("SettingsPage_Profile_Bio_Dialog/Title"),
                Content = textBox,
                PrimaryButtonText = stringsService.GetString("Ok"),
                CloseButtonText = stringsService.GetString("Cancel"),
                DefaultButton = ContentDialogButton.Primary
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                await accountViewModel.UpdateBioCommand.ExecuteAsync(textBox.Text);
            }
        }

        #region Event Handlers

        private void Profile_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.Caption = stringsService.GetString("SettingsPage_Profile_Drag");
        }

        private async void Profile_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.AvailableFormats.Contains(StandardDataFormats.StorageItems))
            {
                var items = from item in await e.DataView.GetStorageItemsAsync()
                            where item.IsOfType(StorageItemTypes.File)
                            select item as IStorageFile;

                if (items.Any())
                {
                    var file = items.First();
                    using var stream = await file.OpenStreamForReadAsync();
                    await accountViewModel.UpdateAvatarCommand.ExecuteAsync(stream);
                }
            }
            else if (e.DataView.AvailableFormats.Contains(StandardDataFormats.Bitmap))
            {
                var bitmap = await e.DataView.GetBitmapAsync();
                using var stream = await bitmap.OpenReadAsync();
                await accountViewModel.UpdateAvatarCommand.ExecuteAsync(stream.AsStream());
            }
        }

        private void Emails_Loaded(object sender, RoutedEventArgs e) => viewModel.LoadEmailsCommand.ExecuteAsync(null);

        private void Accelerators_AddEmail(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) => viewModel.AddEmailCommand.ExecuteAsync(null);

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

        #endregion
    }
}
