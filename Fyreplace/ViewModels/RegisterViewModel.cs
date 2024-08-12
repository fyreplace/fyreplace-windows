using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Events;
using Fyreplace.Services;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class RegisterViewModel : AccountViewModelBase
    {
        public override bool CanSubmit => IsUsernameValid && IsEmailValid;

        public bool IsUsernameValid => !string.IsNullOrWhiteSpace(Username)
            && Username.Length >= 3
            && Username.Length <= 50;

        public bool IsEmailValid => Email.Contains('@')
            && Email.Length >= 3
            && Email.Length <= 254;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public string username = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public string email = "";

        protected override IEvent? Handle(ApiException exception) => new FailureEvent();

        public override Task Submit() => Task.CompletedTask;
    }
}
