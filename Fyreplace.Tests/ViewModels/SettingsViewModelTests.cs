using Fyreplace.Events;
using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    public sealed partial class SettingsViewModelTests : TestsBase
    {
        [TestMethod]
        public async Task LoadingEmailsProducesNoFailures()
        {
            var eventBus = GetEventBus();
            var viewModel = new SettingsViewModel();

            await viewModel.LoadEmailsAsync();
            Assert.AreEqual(0, eventBus.Events.Count(e => e is FailureEvent));
            Assert.AreEqual(3, viewModel.Emails.Count);
        }
    }
}
