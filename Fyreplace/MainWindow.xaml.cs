using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources;

namespace Fyreplace
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = ResourceLoader.GetForViewIndependentUse().GetString("AppName");
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.SetIcon(@"Assets\Icon.ico");
            SetTitleBar(MainPage.GetTitleBar());
        }

        #region Event Handlers

        private void Root_Activated(object sender, WindowActivatedEventArgs args)
        {
            var windowDeactivated = args.WindowActivationState == WindowActivationState.Deactivated;
            MainPage.SetIsTitleBarActive(!windowDeactivated);
        }

        #endregion
    }
}
