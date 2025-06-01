using Fyreplace.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed partial class MainViewModelTests : TestsBase
    {
        [TestMethod]
        public async Task InvalidRandomCodeProducesFailure()
        {
            var eventBus = GetEventBus();
            var viewModel = new MainWindowViewModel();
            var email = FakeApiClient.MakeEmail(verified: false);
            await viewModel.CompleteEmailVerificationAsync(email.Email1, FakeApiClient.badSecret);
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
            Assert.IsFalse(email.Verified);
        }

        [TestMethod]
        public async Task ValidRandomCodeProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var viewModel = new MainWindowViewModel();
            var email = FakeApiClient.MakeEmail(verified: false);
            await viewModel.CompleteEmailVerificationAsync(email.Email1, FakeApiClient.goodSecret);
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
            Assert.IsFalse(email.Verified);
        }
    }
}
