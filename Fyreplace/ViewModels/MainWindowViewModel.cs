using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class MainWindowViewModel: ViewModelBase
    {
        [ObservableProperty]
        private bool showConnectionTip;

        private AccountEntryViewModelBase AccountEntryViewModel => preferences.Account_IsRegistering
                ? AppBase.GetService<RegisterViewModel>()
                : AppBase.GetService<LoginViewModel>();

        public async Task CompleteConnection(string randomCode)
        {
            if (!preferences.Account_IsWaitingForRandomCode)
            {
                return;
            }

            AccountEntryViewModel.RandomCode = randomCode;
            ShowConnectionTip = true;
            await AccountEntryViewModel.SubmitCommand.ExecuteAsync(null);
            ShowConnectionTip = false;
        }
    }
}
