using Fyreplace.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    public sealed partial class AccountViewModelTests : TestsBase
    {
        [TestMethod]
        public async Task ViewModelRetrievesCurrentUser()
        {
            var secrets = GetSecrets();
            var viewModel = new AccountViewModel();

            secrets.Token = FakeApiClient.token;
            await Task.Delay(100);
            Assert.IsNotNull(viewModel.CurrentUser);
        }

        [TestMethod]
        public async Task TooLargeAvatarProducesFailure()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = new AccountViewModel();

            secrets.Token = FakeApiClient.token;
            await Task.Delay(100);
            await viewModel.UpdateAvatarAsync(FakeApiClient.LargeImageStream);
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
            Assert.AreEqual("", viewModel.CurrentUser?.Avatar);
        }

        [TestMethod]
        public async Task NotImageAvatarProducesFailure()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = new AccountViewModel();

            secrets.Token = FakeApiClient.token;
            await Task.Delay(100);
            await viewModel.UpdateAvatarAsync(FakeApiClient.NotImageStream);
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
            Assert.AreEqual("", viewModel.CurrentUser?.Avatar);
        }

        [TestMethod]
        public async Task ValidAvatarProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = new AccountViewModel();

            secrets.Token = FakeApiClient.token;
            await Task.Delay(100);
            await viewModel.UpdateAvatarAsync(FakeApiClient.NormalImageStream);
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
            Assert.AreEqual(FakeApiClient.avatar, viewModel.CurrentUser?.Avatar);
        }
    }
}
