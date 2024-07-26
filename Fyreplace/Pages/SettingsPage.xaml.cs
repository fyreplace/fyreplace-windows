using Fyreplace.Config;
using Microsoft.UI.Xaml.Controls;

namespace Fyreplace.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private readonly BuildInfo config = AppBase.GetService<BuildInfo>();
        private readonly string appVersion = AppBase.GetService<BuildInfo>().Version.Main;

        public SettingsPage() => InitializeComponent();
    }
}
