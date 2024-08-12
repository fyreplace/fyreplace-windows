using Fyreplace.Events;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Threading.Tasks;

namespace Fyreplace.Views
{
    public sealed partial class MainWindow : Window
    {
        private readonly IEventBus EventBus = AppBase.GetService<IEventBus>();

        public MainWindow()
        {
            InitializeComponent();
            var resourceLoader = new ResourceLoader();
            Title = resourceLoader.GetString("AppName");
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.SetIcon(@"Assets\Icon.ico");
            SetTitleBar(MainPage.GetTitleBar());
            EventBus.Subscribe<FailureEvent>(OnFailureEvent);
        }

        #region Event Handlers

        private void Root_Activated(object sender, WindowActivatedEventArgs args) => MainPage.SetIsTitleBarActive(args.WindowActivationState != WindowActivationState.Deactivated);

        private async Task OnFailureEvent(FailureEvent e)
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
    }
}
