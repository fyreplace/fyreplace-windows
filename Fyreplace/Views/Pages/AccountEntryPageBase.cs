using Fyreplace.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics.CodeAnalysis;

namespace Fyreplace.Views.Pages
{
    public abstract class AccountEntryPageBase<VM> : Page where VM : AccountEntryViewModelBase
    {
        protected abstract VM ViewModel { get; }

        [SuppressMessage("Style", "IDE0060", Justification = "Needs to conform to the callback interface")]
        protected void Accelerators_Submit(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (ViewModel.CanSubmit)
            {
                ViewModel.SubmitAsync();
            }
        }
    }
}
