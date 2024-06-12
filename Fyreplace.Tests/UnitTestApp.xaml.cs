using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;

namespace Fyreplace.Tests
{
    public partial class UnitTestApp : Application
    {
        public MainWindow? window;

        public UnitTestApp() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();
            window = new MainWindow();
            window.Activate();
            UITestMethodAttribute.DispatcherQueue = window.DispatcherQueue;
            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
        }
    }
}
