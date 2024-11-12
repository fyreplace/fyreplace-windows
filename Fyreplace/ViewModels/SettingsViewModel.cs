using Fyreplace.Collections;
using Fyreplace.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<Email> Emails => emails;

        private readonly PagingCollection<Email> emails;

        public SettingsViewModel() => emails = new(api.ListEmailsAsync);

        public async Task LoadEmailsAsync()
        {
            bool? hasMore;

            do
            {
                hasMore = await CallAsync(emails.FetchMoreAsync);
            }
            while (hasMore ?? false);
        }
    }
}
