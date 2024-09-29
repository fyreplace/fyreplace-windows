using Fyreplace.Events;
using Fyreplace.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Fyreplace.Views
{
    public sealed partial class MainWindow : Window
    {
        private readonly IEventBus EventBus = AppBase.GetService<IEventBus>();
        private readonly MainWindowViewModel viewModel = AppBase.GetService<MainWindowViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            var resourceLoader = new ResourceLoader();
            Title = resourceLoader.GetString("AppName");
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.SetIcon(@"Assets\Icon.ico");
            SetTitleBar(MainPage.GetTitleBar());
            EventBus.Subscribe<FailureEvent>(OnFailureEventAsync);
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

        private void Root_Activated(object sender, WindowActivatedEventArgs args) => MainPage.SetIsTitleBarActive(args.WindowActivationState != WindowActivationState.Deactivated);

        private async Task OnFailureEventAsync(FailureEvent e)
        {
            var resourceLoader = new ResourceLoader();
            var dialog = new ContentDialog
            {
                XamlRoot = MainPage.XamlRoot,
                Title = resourceLoader.GetString(e.Title),
                CloseButtonText = resourceLoader.GetString("Ok"),
                DefaultButton = ContentDialogButton.Close,
                Content = resourceLoader.GetString(e.Message)
            };

            await dialog.ShowAsync();
        }

        #endregion

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SwitchToThisWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool fAltTab);
    }
}
