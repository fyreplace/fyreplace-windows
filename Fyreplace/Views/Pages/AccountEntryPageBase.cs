using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace Fyreplace.Views.Pages
{
    public abstract class AccountEntryPageBase<VM> : Page where VM : AccountEntryViewModelBase
    {
        protected abstract IDictionary<string, UIElement> ConnectedElements { get; }

        protected abstract VM viewModel { get; }

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

        protected void Accelerators_Submit(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (viewModel.CanSubmit)
            {
                viewModel.SubmitAsync();
            }
        }
    }
}
