using Fyreplace.Config;
using Fyreplace.Events;
using Fyreplace.Services;
using Fyreplace.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Fyreplace.Views
{
    public sealed partial class MainWindow : Window, IImageFileService
    {
        private readonly IEventBus eventBus = App.GetService<IEventBus>();
        private readonly IStringsService stringsService = App.GetService<IStringsService>();
        private readonly MainWindowViewModel viewModel = App.GetService<MainWindowViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            Title = App.GetService<BuildInfo>().App.Name;
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.SetTitleBarIcon(@"Assets\Icon.ico");
            SetTitleBar(MainPage.GetTitleBar());
            eventBus.Subscribe<FailureEvent>(OnFailureEventAsync);
        }

        public void Show() => SwitchToThisWindow(WindowNative.GetWindowHandle(this), false);

        public async Task<StorageFile?> PickImageFileAsync()
        {
            var picker = new FileOpenPicker();
            var handle = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, handle);
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".wepb");
            return await picker.PickSingleFileAsync();
        }

        #region Event Handlers

        private async Task OnFailureEventAsync(FailureEvent e)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = MainPage.XamlRoot,
                Title = stringsService.GetString(e.Title),
                CloseButtonText = stringsService.GetString("Ok"),
                DefaultButton = ContentDialogButton.Close,
                Content = stringsService.GetString(e.Message)
            };
            await dialog.ShowAsync();
        }

        #endregion

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SwitchToThisWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool fAltTab);
    }
}
