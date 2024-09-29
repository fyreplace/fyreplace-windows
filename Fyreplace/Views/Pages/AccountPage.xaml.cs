using CommunityToolkit.Mvvm.Input;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fyreplace.Views.Pages
{
    public sealed partial class AccountPage : Page
    {
        private readonly AccountViewModel viewModel = AppBase.GetService<AccountViewModel>();

        public AccountPage() => InitializeComponent();

        [RelayCommand]
        private async Task UpdateAvatarAsync()
        {
            var file = await AppBase.GetService<MainWindow>().PickImageFileAsync();

            if (file != null)
            {
                using var stream = await file.OpenReadAsync();
                await viewModel.UpdateAvatarCommand.ExecuteAsync(stream.AsStream());
            }
        }
    }
}
