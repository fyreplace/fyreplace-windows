using Fyreplace.Config;
using Microsoft.UI.Xaml.Controls;

namespace Fyreplace.Views.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private readonly string appVersion = AppBase.GetService<BuildInfo>().Version.Main;

        public SettingsPage() => InitializeComponent();
    }
}
