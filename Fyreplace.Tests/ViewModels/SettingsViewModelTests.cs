using Fyreplace.Events;
using Fyreplace.Tests.Services;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            Assert.DoesNotContain(e => e is FailureEvent, eventBus.Events);
            Assert.HasCount(3, viewModel.Emails);
        }

        [TestMethod]
        public async Task InvalidEmailProducesFailure()
        {
            var eventBus = GetEventBus();
            var viewModel = UnitTestApp.GetService<SettingsViewModel>();
            viewModel.NewEmail = FakeApiClient.badEmail;
            await viewModel.AddEmailAsync();
            Assert.ContainsSingle(e => e is FailureEvent, eventBus.Events);
            Assert.HasCount(0, viewModel.Emails);
        }

        [TestMethod]
        public async Task ValidEmailProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var viewModel = UnitTestApp.GetService<SettingsViewModel>();
            viewModel.NewEmail = FakeApiClient.goodEmail;
            await viewModel.AddEmailAsync();
            Assert.DoesNotContain(e => e is FailureEvent, eventBus.Events);
            Assert.HasCount(1, viewModel.Emails);
        }
    }
}
