using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;

namespace Fyreplace.ViewModels
{
    public sealed partial class AccountViewModel : ViewModelBase
    {
        [RelayCommand]
        public void Logout() => secrets.Token = string.Empty;
    }
}
