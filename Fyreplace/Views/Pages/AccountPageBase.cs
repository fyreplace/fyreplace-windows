using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace Fyreplace.Views.Pages
{
    public abstract class AccountPageBase : Page
    {
        protected abstract IDictionary<string, UIElement> ConnectedElements { get; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var animator = ConnectedAnimationService.GetForCurrentView();

            foreach (var entry in ConnectedElements)
            {
                animator.GetAnimation(entry.Key)?.TryStart(entry.Value);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var animator = ConnectedAnimationService.GetForCurrentView();

            foreach (var entry in ConnectedElements)
            {
                animator
                    .PrepareToAnimate(entry.Key, entry.Value)
                    .Configuration = new DirectConnectedAnimationConfiguration();
            }

            base.OnNavigatingFrom(e);
        }
    }
}
