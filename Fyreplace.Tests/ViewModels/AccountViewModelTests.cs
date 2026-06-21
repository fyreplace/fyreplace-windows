using Fyreplace.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed partial class AccountViewModelTests : TestsBase
    {
        [TestMethod]
        public async Task ViewModelRetrievesCurrentUser()
        {
            var secrets = GetSecrets();
            var viewModel = UnitTestApp.GetService<AccountViewModel>();
            secrets.Token = FakeApiClient.token;
            await Task.Delay(100, TestContext.CancellationToken);
            Assert.IsNotNull(viewModel.CurrentUser);
        }

        [TestMethod]
        public async Task UpdateAvatarTooLargeProducesFailure()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = UnitTestApp.GetService<AccountViewModel>();
            secrets.Token = FakeApiClient.token;
            await viewModel.UpdateAvatarAsync(FakeApiClient.LargeImageStream);
            Assert.ContainsSingle(e => e is FailureEvent, eventBus.Events);
            Assert.AreEqual(string.Empty, viewModel.CurrentUser?.Avatar);
        }

        [TestMethod]
        public async Task UpdateAvatarNotImageProducesFailure()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = UnitTestApp.GetService<AccountViewModel>();
            secrets.Token = FakeApiClient.token;
            await viewModel.UpdateAvatarAsync(FakeApiClient.NotImageStream);
            Assert.ContainsSingle(e => e is FailureEvent, eventBus.Events);
            Assert.AreEqual(string.Empty, viewModel.CurrentUser?.Avatar);
        }

        [TestMethod]
        public async Task UpdateAvatarValidProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = UnitTestApp.GetService<AccountViewModel>();
            secrets.Token = FakeApiClient.token;
            await viewModel.UpdateAvatarAsync(FakeApiClient.NormalImageStream);
            Assert.DoesNotContain(e => e is FailureEvent, eventBus.Events);
            Assert.AreEqual(FakeApiClient.avatar, viewModel.CurrentUser?.Avatar);
        }

        [TestMethod]
        public async Task RemoveAvatarProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = UnitTestApp.GetService<AccountViewModel>();
            secrets.Token = FakeApiClient.token;
            await viewModel.UpdateAvatarAsync(FakeApiClient.NormalImageStream);
            await viewModel.RemoveAvatarAsync();
            Assert.DoesNotContain(e => e is FailureEvent, eventBus.Events);
            Assert.AreEqual(string.Empty, viewModel.CurrentUser?.Avatar);
        }

        [TestMethod]
        public async Task UpdateBioProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var secrets = GetSecrets();
            var viewModel = UnitTestApp.GetService<AccountViewModel>();
            secrets.Token = FakeApiClient.token;
            await viewModel.UpdateBioAsync("Hello");
            Assert.DoesNotContain(e => e is FailureEvent, eventBus.Events);
            Assert.AreEqual("Hello", viewModel.CurrentUser?.Bio);
        }

        public TestContext TestContext { get; set; }
    }
}
