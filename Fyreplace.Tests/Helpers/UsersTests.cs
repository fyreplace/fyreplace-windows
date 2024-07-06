using Fyreplace.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fyreplace.Tests.Helpers
{
    [TestClass]
    public class UsersTests
    {
        [TestMethod]
        public void TestUsernameMustHaveCorrectLength()
        {
            for (int i = 3; i <= 50; i++)
            {
                Assert.IsTrue(Users.IsUsernameValid(new string('a', i)));
            }

            for (int i = 0; i < 3; i++)
            {
                Assert.IsFalse(Users.IsUsernameValid(new string('a', i)));
            }

            Assert.IsFalse(Users.IsUsernameValid(new string('a', 51)));
        }

        [TestMethod]
        public void TestEmailMustHaveCorrectLength()
        {
            for (int i = 3; i <= 254; i++)
            {
                Assert.IsTrue(Users.IsEmailValid(new string('@', i)));
            }

            for (int i = 0; i < 3; i++)
            {
                Assert.IsFalse(Users.IsEmailValid(new string('@', i)));
            }

            Assert.IsFalse(Users.IsEmailValid(new string('@', 255)));
        }

        [TestMethod]
        public void TestEmailMustHaveAtSign()
        {
            Assert.IsFalse(Users.IsEmailValid("email"));
            Assert.IsTrue(Users.IsEmailValid("email@example"));
        }
    }
}
