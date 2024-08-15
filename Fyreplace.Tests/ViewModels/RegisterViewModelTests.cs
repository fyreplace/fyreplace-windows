using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
