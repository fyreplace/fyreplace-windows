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

            preferences.Account_Identifier = FakeApiClient.badUsername;
            await viewModel.Submit();
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
            Assert.IsFalse(preferences.Account_IsWaitingForRandomCode);
        }

        [TestMethod]
        public async Task ValidIdentifierProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();

            preferences.Account_Identifier = FakeApiClient.goodUsername;
            await viewModel.Submit();
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
            Assert.IsTrue(preferences.Account_IsWaitingForRandomCode);
        }

        [TestMethod]
        public async Task PasswordIdentifierProducesFailure()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();

            preferences.Account_Identifier = FakeApiClient.passwordUsername;
            await viewModel.Submit();
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
            Assert.IsTrue(preferences.Account_IsWaitingForRandomCode);
        }

        [TestMethod]
        public void RandomCodeMustHaveCorrectLength()
        {
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();
            preferences.Account_IsWaitingForRandomCode = true;

            viewModel.RandomCode = "abcd123";
            Assert.IsFalse(viewModel.CanSubmit);
            viewModel.RandomCode = "abcd1234";
            Assert.IsTrue(viewModel.CanSubmit);
        }

        [TestMethod]
        public async Task InvalidRandomCodeProducesFailure()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();
            preferences.Account_Identifier = FakeApiClient.goodUsername;
            preferences.Account_IsWaitingForRandomCode = true;

            viewModel.RandomCode = FakeApiClient.badSecret;
            await viewModel.Submit();
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
        }

        [TestMethod]
        public async Task ValidRandomCodeProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new LoginViewModel();
            preferences.Account_Identifier = FakeApiClient.goodUsername;
            preferences.Account_IsWaitingForRandomCode = true;

            viewModel.RandomCode = FakeApiClient.goodSecret;
            await viewModel.Submit();
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
        }
    }
}
