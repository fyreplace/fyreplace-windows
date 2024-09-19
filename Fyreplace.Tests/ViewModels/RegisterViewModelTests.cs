using Fyreplace.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed class RegisterViewModelTests : TestsBase
    {
        [TestMethod]
        public void UsernameMustHaveCorrectLength()
        {
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();
            preferences.Account_Email = "email@example";

            for (int i = 3; i <= 50; i++)
            {
                preferences.Account_Username = new string('a', i);
                Assert.IsTrue(viewModel.CanSubmit);
            }

            for (int i = 0; i < 3; i++)
            {
                preferences.Account_Username = new string('a', i);
                Assert.IsFalse(viewModel.CanSubmit);
            }

            preferences.Account_Username = new string('a', 51);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void EmailMustHaveCorrectLength()
        {
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();
            preferences.Account_Username = "Example";

            for (int i = 3; i <= 254; i++)
            {
                preferences.Account_Email = new string('@', i);
                Assert.IsTrue(viewModel.CanSubmit);
            }

            for (int i = 0; i < 3; i++)
            {
                preferences.Account_Email = new string('@', i);
                Assert.IsFalse(viewModel.CanSubmit);
            }

            preferences.Account_Email = new string('@', 255);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void EmailMustHaveAtSign()
        {
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();

            preferences.Account_Username = "Example";
            preferences.Account_Email = "email";
            Assert.IsFalse(viewModel.CanSubmit);

            preferences.Account_Email = "email@example";
            Assert.IsTrue(viewModel.CanSubmit);
        }

        [TestMethod]
        public async Task InvalidUsernameProducesFailure()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();
            preferences.Account_Email = FakeApiClient.goodEmail;
            var invalidValues = new List<string> { FakeApiClient.badUsername, FakeApiClient.reservedUsername, FakeApiClient.usedUsername };

            for (var i = 0; i < invalidValues.Count; i++)
            {
                preferences.Account_Username = invalidValues[i];
                await viewModel.SubmitAsync();
                Assert.AreEqual(i + 1, eventBus.Events.Count(e => e is FailureEvent));
                Assert.IsFalse(preferences.Account_IsWaitingForRandomCode);
            }
        }

        [TestMethod]
        public async Task InvalidEmailProducesFailure()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();
            preferences.Account_Username = FakeApiClient.goodUsername;
            var invalidValues = new List<string> { FakeApiClient.badEmail, FakeApiClient.usedEmail };

            for (var i = 0; i < invalidValues.Count; i++)
            {
                preferences.Account_Email = invalidValues[i];
                await viewModel.SubmitAsync();
                Assert.AreEqual(i + 1, eventBus.Events.Count(e => e is FailureEvent));
                Assert.IsFalse(preferences.Account_IsWaitingForRandomCode);
            }
        }

        [TestMethod]
        public async Task ValidUsernameAndEmailProduceNoFailures()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();

            preferences.Account_Username = FakeApiClient.goodUsername;
            preferences.Account_Email = FakeApiClient.goodEmail;
            await viewModel.SubmitAsync();
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
            Assert.IsTrue(preferences.Account_IsWaitingForRandomCode);
        }

        [TestMethod]
        public void RandomCodeMustHaveCorrectLength()
        {
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();
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
            var viewModel = new RegisterViewModel();
            preferences.Account_Username = FakeApiClient.goodUsername;
            preferences.Account_Email = FakeApiClient.goodEmail;
            preferences.Account_IsWaitingForRandomCode = true;

            viewModel.RandomCode = FakeApiClient.badSecret;
            await viewModel.SubmitAsync();
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
        }

        [TestMethod]
        public async Task ValidRandomCodeProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var preferences = GetPreferences();
            var viewModel = new RegisterViewModel();
            preferences.Account_Username = FakeApiClient.goodUsername;
            preferences.Account_Email = FakeApiClient.goodEmail;
            preferences.Account_IsWaitingForRandomCode = true;

            viewModel.RandomCode = FakeApiClient.goodSecret;
            await viewModel.SubmitAsync();
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
        }
    }
}
