using Fyreplace.Events;
using Fyreplace.Tests.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed class LoginViewModelTests
    {
        [TestInitialize]
        public void TestInitialize() => GetEventBus().Clear();

        [TestMethod]
        public void IdentifierMustHaveCorrectLength()
        {
            var viewModel = new LoginViewModel();

            for (int i = 3; i <= 254; i++)
            {
                viewModel.Identifier = new string('a', i);
                Assert.IsTrue(viewModel.CanSubmit);
            }

            for (int i = 0; i < 3; i++)
            {
                viewModel.Identifier = new string('a', i);
                Assert.IsFalse(viewModel.CanSubmit);
            }

            viewModel.Identifier = new string('a', 255);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public async Task InvalidIdentifierProducesFailure()
        {
            var eventBus = GetEventBus();
            var viewModel = new LoginViewModel
            {
                Identifier = FakeApiClient.BadIdentifier
            };

            await viewModel.Submit();
            Assert.AreEqual(1, eventBus.Events.Count);
            Assert.IsInstanceOfType<FailureEvent>(eventBus?.Events[0]);
        }

        [TestMethod]
        public async Task ValidIdentifierProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var viewModel = new LoginViewModel
            {
                Identifier = FakeApiClient.GoodIdentifier
            };

            await viewModel.Submit();
            Assert.AreEqual(0, eventBus.Events.Count);
        }

        private static StoringEventBus GetEventBus() => (StoringEventBus)AppBase.GetService<IEventBus>();
    }
}
