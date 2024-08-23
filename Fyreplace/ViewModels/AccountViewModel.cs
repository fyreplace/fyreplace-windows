using CommunityToolkit.Mvvm.Input;
using Fyreplace.Data;

namespace Fyreplace.ViewModels
{
    public sealed partial class AccountViewModel : ViewModelBase
    {
        private readonly IPreferences preferences = AppBase.GetService<IPreferences>();

        [RelayCommand]
        public void Logout() => secrets.Token = string.Empty;
    }
}
