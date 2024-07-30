using Fyreplace.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fyreplace.Tests.ViewModels
{
    [TestClass]
    public sealed class LoginViewModelTests
    {
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
    }
}
