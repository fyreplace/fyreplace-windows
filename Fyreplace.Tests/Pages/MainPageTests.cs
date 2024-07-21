using CommunityToolkit.WinUI;
using Fyreplace.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Linq;

namespace Fyreplace.Tests.Pages
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

        [UITestMethod]
        public void TestNavigationItemsAreUnique()
        {
            var navigation = GetNavigation();
            var items = navigation.MenuItems.OfType<NavigationViewItem>();
            
            foreach (var first in items)
            {
                foreach (var second in items)
                {
                    if (first != second)
                    {
                        Assert.AreNotEqual(first.Content, second.Content);
                        Assert.AreNotEqual(first.Icon, second.Icon);
                        Assert.AreNotEqual(first.Tag, second.Tag);
                    }
                }
            }
        }

        private static NavigationView GetNavigation()
        {
            var window = AppBase.GetService<MainWindow>();
            var mainPage = window.Content as MainPage;
            var navigation = mainPage?.FindChild("Navigation") as NavigationView;
            Assert.IsNotNull(navigation);
            return navigation;
        }
    }
}
