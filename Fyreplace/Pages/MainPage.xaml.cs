using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.Graphics;

namespace Fyreplace.Pages
{
    public sealed partial class MainPage : Page
    {
        public AppWindow? AppWindow { get; set; }

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
            if (AppWindow == null)
            {
                return;
            }

            var scale = XamlRoot.RasterizationScale;
            TitleBarLeftPadding.Width = new GridLength((AppWindow.TitleBar.LeftInset) / scale);
            TitleBarRightPadding.Width = new GridLength((AppWindow.TitleBar.RightInset) / scale);

            var transform = Avatar.TransformToVisual(null);
            var bounds = transform.TransformBounds(new Rect(0, 0, Avatar.ActualWidth, Avatar.ActualHeight));
            var avatarRect = new RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
            var nonClientInput = InputNonClientPointerSource.GetForWindowId(AppWindow.Id);
            nonClientInput.SetRegionRects(NonClientRegionKind.Passthrough, [avatarRect]);
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

        private void Avatar_Click(object sender, RoutedEventArgs e) => FlyoutBase.ShowAttachedFlyout(Avatar);

        private void AccountSelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args) => AccountHost.Navigate(
                sender.SelectedItem == Login ? typeof(LoginPage) : typeof(RegisterPage),
                null,
                new SuppressNavigationTransitionInfo()
            );

        #endregion
    }
}
