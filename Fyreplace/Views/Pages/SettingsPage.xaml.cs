using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
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
        private readonly string appVersion = AppBase.GetService<BuildInfo>().Version.Main;
        private readonly IEventBus eventBus = AppBase.GetService<IEventBus>();
        private readonly ISecrets secrets = AppBase.GetService<ISecrets>();
        private readonly AccountViewModel accountViewModel = AppBase.GetService<AccountViewModel>();

        public SettingsPage()
        {
            InitializeComponent();
            eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);
        }

        #region Event Handlers

        private void Profile_DragOver(object sender, DragEventArgs e)
        {
            var resources = new ResourceLoader();
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.Caption = resources.GetString("SettingsPage_Profile_Drag");
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
