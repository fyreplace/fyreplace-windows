using Fyreplace.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed class LoginViewModelTests : TestsBase
    {
        [TestMethod]
        public void IdentifierMustHaveCorrectLength()
        {
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();

            for (int i = 3; i <= 254; i++)
            {
                preferences.Account_Identifier = new string('a', i);
                Assert.IsTrue(viewModel.CanSubmit);
            }

            for (int i = 0; i < 3; i++)
            {
                preferences.Account_Identifier = new string('a', i);
                Assert.IsFalse(viewModel.CanSubmit);
            }

            preferences.Account_Identifier = new string('a', 255);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public async Task InvalidIdentifierProducesFailure()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();

            preferences.Account_Identifier = FakeApiClient.BadIdentifier;
            await viewModel.Submit();
            Assert.IsFalse(preferences.Account_IsWaitingForRandomCode);
            Assert.AreEqual(1, eventBus.Events.Where(e => e is FailureEvent).Count());
        }

        [TestMethod]
        public async Task ValidIdentifierProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();

            preferences.Account_Identifier = FakeApiClient.GoodIdentifier;
            await viewModel.Submit();
            Assert.IsTrue(preferences.Account_IsWaitingForRandomCode);
            Assert.AreEqual(0, eventBus.Events.Where(e => e is FailureEvent).Count());
        }

        [TestMethod]
        public void RandomCodeMustHaveCorrectLength()
        {
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();
            preferences.Account_Identifier = FakeApiClient.GoodIdentifier;
            preferences.Account_IsWaitingForRandomCode = true;
            viewModel.RandomCode = "12345";
            Assert.IsFalse(viewModel.CanSubmit);
            viewModel.RandomCode = "123456";
            Assert.IsTrue(viewModel.CanSubmit);
            viewModel.RandomCode = "1234567";
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public async Task InvalidRandomCodeProducesFailure()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();

            preferences.Account_Identifier = FakeApiClient.GoodIdentifier;
            preferences.Account_IsWaitingForRandomCode = true;
            viewModel.RandomCode = FakeApiClient.BadSecret;
            await viewModel.Submit();
            Assert.AreEqual(1, eventBus.Events.Where(e => e is FailureEvent).Count());
        }

        [TestMethod]
        public async Task ValidRandomCodeProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();

            preferences.Account_Identifier = FakeApiClient.GoodIdentifier;
            preferences.Account_IsWaitingForRandomCode = true;
            viewModel.RandomCode = FakeApiClient.GoodSecret;
            await viewModel.Submit();
            Assert.AreEqual(0, eventBus.Events.Where(e => e is FailureEvent).Count());
        }
    }
}
