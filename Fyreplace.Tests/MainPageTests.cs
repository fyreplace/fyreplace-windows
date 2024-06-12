using CommunityToolkit.WinUI;
using Fyreplace.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Linq;

namespace Fyreplace.Tests
{
    [TestClass]
    public class MainPageTests
    {
        [UITestMethod]
        public void TestInitialPageIsFeed()
        {
            var navigation = GetNavigation();
            var items = navigation.MenuItems.OfType<NavigationViewItem>();
            Assert.AreEqual("FeedPage", items.First().Tag);
            Assert.IsTrue(items.First().IsSelected);
            var host = navigation.Content as Frame;
            Assert.IsNotNull(host);
            Assert.IsInstanceOfType<FeedPage>(host.Content);
        }

        [UITestMethod]
        public void TestNavigationIsComplete()
        {
            var navigation = GetNavigation();
            var items = navigation.MenuItems.OfType<NavigationViewItem>();
            Assert.AreEqual(5, items.Count());
        }

        private static NavigationView GetNavigation()
        {
            var app = Application.Current as UnitTestApp;
            var mainPage = app?.window?.Content as MainPage;
            var navigation = mainPage?.FindChild("Navigation") as NavigationView;
            Assert.IsNotNull(navigation);
            return navigation;
        }
    }
}
