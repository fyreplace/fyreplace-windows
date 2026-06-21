using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Fyreplace.Config;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.ViewModels;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Fyreplace.Views.Pages
{
    public sealed partial class MainPage : Page
    {
        public AppWindow? AppWindow { get; set; }

        [SuppressMessage("Performance", "CA1822", Justification = "Used in the XAML page")]
        public string AppName => App.GetService<BuildInfo>().App.Name;

        private NavigationViewItemBase? currentInvokedItem;
        private readonly ISecrets secrets = App.GetService<ISecrets>();
        private readonly IEventBus eventBus = App.GetService<IEventBus>();
        private readonly AccountViewModel accountViewModel = App.GetService<AccountViewModel>();

        private static readonly Type[] navigablePageTypes = [
            typeof(FeedPage),
            typeof(NotificationsPage),
            typeof(ArchivePage),
            typeof(DraftsPage),
            typeof(SettingsPage),
        ];

        public MainPage()
        {
            InitializeComponent();
            currentInvokedItem = Feed;
            Host.Navigate(typeof(FeedPage), null, new SuppressNavigationTransitionInfo());
            eventBus.Subscribe<SecretChangedEvent>(OnSecretChangedAsync);
        }

        #region Title Bar

        public UIElement GetTitleBar() => TitleBar;

        #endregion

        #region Navigation

        [RelayCommand]
        public void GoToSettings()
        {
            if (Navigation.SelectedItem != Navigation.SettingsItem)
            {
                NavigatePoppingBackStack(typeof(SettingsPage));
            }
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

        private void NavigatePoppingBackStack(Type pageType)
        {
            Host.Navigate(pageType, null, new ContinuumNavigationTransitionInfo());
            Host.BackStack.Clear();
            currentInvokedItem = Navigation.SelectedItem as NavigationViewItemBase;
        }

        private void UpdateNavigationSelection() => Navigation.SelectedItem = Navigation.MenuItems
            .OfType<NavigationViewItem>()
            .Where(item => item.Tag.ToString() == Host.CurrentSourcePageType.Name)
            .SingleOrDefault(Navigation.SettingsItem);

        #endregion

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

        private void Accelerators_GoBack(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            GoBack();
            args.Handled = true;
        }

        private void Accelerators_GoForward(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            GoForward();
            args.Handled = true;
        }

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

            NavigatePoppingBackStack(
                args.IsSettingsInvoked
                    ? typeof(SettingsPage)
                    : navigablePageTypes
                        .Where(page => page.Name == (string)args.InvokedItemContainer.Tag)
                        .SingleOrDefault(typeof(ErrorPage))
            );
        }

        private void Navigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => GoBack();

        private void Host_Navigated(object sender, NavigationEventArgs e) => UpdateNavigationSelection();

        private void Host_NavigationFailed(object sender, NavigationFailedEventArgs e) => Host.Navigate(typeof(ErrorPage), e.Exception);

        [RelayCommand]
        public void ShowAccountFlyout() => FlyoutBase.ShowAttachedFlyout(string.IsNullOrEmpty(secrets.Token) ? AvatarWrapper : Avatar);

        private Task OnSecretChangedAsync(SecretChangedEvent e)
        {
            switch (e.Name)
            {
                case nameof(ISecrets.Token):
                    if (!string.IsNullOrEmpty(secrets.Token))
                    {
                        return DispatcherQueue.EnqueueAsync(() => FlyoutBase.GetAttachedFlyout(AvatarWrapper).Hide());
                    }
                    else if (Navigation.SelectedItem != (object)Feed && Navigation.SelectedItem != Navigation.SettingsItem)
                    {
                        NavigatePoppingBackStack(typeof(FeedPage));
                    }

                    break;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
