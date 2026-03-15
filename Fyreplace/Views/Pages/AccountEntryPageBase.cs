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
        protected abstract VM ViewModel { get; }

        protected void Accelerators_Submit(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (ViewModel.CanSubmit)
            {
                ViewModel.SubmitAsync();
            }
        }
    }
}
