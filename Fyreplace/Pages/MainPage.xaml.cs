using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using System.Reflection;

namespace Fyreplace.Pages
{
    public sealed partial class MainPage : Page
    {
        public AppWindowTitleBar? WindowTitleBar { get; set; }
        private NavigationViewItemBase currentInvokedItem;

        public MainPage()
        {
            InitializeComponent();
            currentInvokedItem = Feed;
            Host.Navigate(typeof(FeedPage));
        }

        public UIElement GetTitleBar() => TitleBar;

        public void SetIsTitleBarActive(bool active) => VisualStateManager.GoToState(this, active ? "Window_Active" : "Window_Inactive", true);

        private void UpdateRegionsForTitleBar()
        {
            TitleBarLeftPadding.Width = new GridLength((WindowTitleBar?.LeftInset ?? 0) / XamlRoot.RasterizationScale);
            TitleBarRightPadding.Width = new GridLength((WindowTitleBar?.RightInset ?? 0) / XamlRoot.RasterizationScale);
        }

        private void GoBack()
        {
            if (Host.CanGoBack)
            {
                Host.GoBack();
            }
        }

        private void GoForward()
        {
            if (Host.CanGoForward)
            {
                Host.GoForward();
            }
        }

        private void UpdateNavigationSelection()
        {
            Navigation.SelectedItem = Navigation.MenuItems
                .OfType<NavigationViewItem>()
                .Where(item => item.Tag.ToString() == Host.CurrentSourcePageType.Name)
                .SingleOrDefault(Navigation.SettingsItem);
        }

        #region Event Handlers

        private void MainPage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;

            if (properties.IsXButton1Pressed)
            {
                GoBack();
            }
            else if (properties.IsXButton2Pressed)
            {
                GoForward();
            }
            else
            {
                return;
            }

            e.Handled = true;
        }

        private void MainPage_Accelerators_GoBack(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            GoBack();
            args.Handled = true;
        }

        private void MainPage_Accelerators_GoForward(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            GoForward();
            args.Handled = true;
        }

        private void TitleBar_Loaded(object sender, RoutedEventArgs e) => UpdateRegionsForTitleBar();

        private void TitleBar_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateRegionsForTitleBar();

        private void Navigation_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            var compactTitleBar = args.DisplayMode == NavigationViewDisplayMode.Minimal;
            VisualStateManager.GoToState(this, compactTitleBar ? "TitleBar_Compact" : "TitleBar_Default", true);
        }

        private void Navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer == currentInvokedItem)
            {
                return;
            }

            currentInvokedItem = args.InvokedItemContainer;
            Host.Navigate(
                args.IsSettingsInvoked
                    ? typeof(SettingsPage)
                    : Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(page => page.Namespace == "Fyreplace.Pages")
                        .Where(page => page.Name == (string)args.InvokedItemContainer.Tag)
                        .SingleOrDefault(typeof(ErrorPage)),
                null,
                new ContinuumNavigationTransitionInfo()
            );
            Host.BackStack.Clear();
        }

        private void Navigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => GoBack();

        private void Host_Navigated(object sender, NavigationEventArgs e) => UpdateNavigationSelection();

        private void Host_NavigationFailed(object sender, NavigationFailedEventArgs e) => Host.Navigate(typeof(ErrorPage), e.Exception);

        #endregion
    }
}
