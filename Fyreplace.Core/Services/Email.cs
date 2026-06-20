using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Fyreplace.Services
{
    public partial class Email : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanVerify))]
        [NotifyCanExecuteChangedFor(nameof(VerifyCommand))]
        public partial string Code { get; set; } = string.Empty;

        public bool CanVerify => !string.IsNullOrWhiteSpace(Code);

        public event EmailVerificationEventHandler? VerificationRequested;

        [RelayCommand(CanExecute = nameof(CanVerify))]
        public Task VerifyAsync() => VerificationRequested?.Invoke(this, new EmailVerificationEventArgs { Code = Code }) ?? Task.CompletedTask;
    }

    public class EmailVerificationEventArgs : EventArgs
    {
        public required string Code { get; set; }
    }

    public delegate Task EmailVerificationEventHandler(Email sender, EmailVerificationEventArgs e);
}
