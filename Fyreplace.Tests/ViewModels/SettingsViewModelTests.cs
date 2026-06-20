using Fyreplace.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed partial class SettingsViewModelTests : TestsBase
    {
        [TestMethod]
        public async Task LoadingEmailsProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var viewModel = UnitTestApp.GetService<SettingsViewModel>();
            await viewModel.LoadEmailsAsync();
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
            Assert.AreEqual(3, viewModel.Emails.Count);
        }

        [TestMethod]
        public async Task InvalidEmailProducesFailure()
        {
            var eventBus = GetEventBus();
            var viewModel = UnitTestApp.GetService<SettingsViewModel>();
            viewModel.NewEmail = FakeApiClient.badEmail;
            await viewModel.AddEmailAsync();
            Assert.AreEqual(1, eventBus.Events.Count(e => e is FailureEvent));
            Assert.AreEqual(0, viewModel.Emails.Count);
        }

        [TestMethod]
        public async Task ValidEmailProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var viewModel = UnitTestApp.GetService<SettingsViewModel>();
            viewModel.NewEmail = FakeApiClient.goodEmail;
            await viewModel.AddEmailAsync();
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
            Assert.AreEqual(1, viewModel.Emails.Count);
        }
    }
}
