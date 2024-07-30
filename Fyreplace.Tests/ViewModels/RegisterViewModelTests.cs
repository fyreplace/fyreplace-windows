using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed class RegisterViewModelTests
    {
        [TestMethod]
        public void UsernameMustHaveCorrectLength()
        {
            var viewModel = new RegisterViewModel { Email = "email@example" };

            for (int i = 3; i <= 50; i++)
            {
                viewModel.Username = new string('a', i);
                Assert.IsTrue(viewModel.CanSubmit);
            }

            for (int i = 0; i < 3; i++)
            {
                viewModel.Username = new string('a', i);
                Assert.IsFalse(viewModel.CanSubmit);
            }

            viewModel.Username = new string('a', 51);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void EmailMustHaveCorrectLength()
        {
            var viewModel = new RegisterViewModel { Username = "Example" };

            for (int i = 3; i <= 254; i++)
            {
                viewModel.Email = new string('@', i);
                Assert.IsTrue(viewModel.CanSubmit);
            }

            for (int i = 0; i < 3; i++)
            {
                viewModel.Email = new string('@', i);
                Assert.IsFalse(viewModel.CanSubmit);
            }

            viewModel.Email = new string('@', 255);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void EmailMustHaveAtSign()
        {
            var viewModel = new RegisterViewModel
            {
                Username = "Example",
                Email = "email"
            };

            Assert.IsFalse(viewModel.CanSubmit);

            viewModel.Email = "email@example";
            Assert.IsTrue(viewModel.CanSubmit);
        }
    }
}
