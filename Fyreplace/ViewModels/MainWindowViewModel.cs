using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class MainWindowViewModel: ViewModelBase
    {
        [ObservableProperty]
        public partial bool ShowUserConnectionTip { get; set; }

        private AccountEntryViewModelBase AccountEntryViewModel => preferences.Account_IsRegistering
                ? AppBase.GetService<RegisterViewModel>()
                : AppBase.GetService<LoginViewModel>();

        public async Task CompleteConnectionAsync(string randomCode)
        {
            if (!preferences.Account_IsWaitingForRandomCode)
            {
                return;
            }

            AccountEntryViewModel.RandomCode = randomCode;
            ShowUserConnectionTip = true;
            await AccountEntryViewModel.SubmitCommand.ExecuteAsync(null);
            ShowUserConnectionTip = false;
        }
    }
}
