using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using System.Net;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public sealed partial class LoginViewModel : AccountViewModelBase
    {
        public override bool CanSubmit => !string.IsNullOrWhiteSpace(Identifier)
            && Identifier.Length >= 3
            && Identifier.Length <= 254
            && !IsLoading;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public string identifier = "";

        private readonly ISettings settings = AppBase.GetService<ISettings>();
        private IApiClient Api => AppBase.GetService<IApiClient>(settings.Environment);

        protected override IEvent? Handle(ApiException exception) => exception.StatusCode switch
        {
            (int)HttpStatusCode.NotFound => new FailureEvent("LoginPage_Error_NotFound_Title", "LoginPage_Error_NotFound_Message"),
            _ => new FailureEvent()
        };

        public override Task Submit() => CallWhileLoading(() => Api.CreateNewTokenAsync(new() { Identifier = Identifier }));
    }
}
