using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.Linq;

namespace Fyreplace.Controls.Settings
{
    public sealed partial class KeyboardShortcut : UserControl
    {
        public string Key { get; set; } = "";

        public bool Ctrl { get; set; } = false;

        public bool Shift { get; set; } = false;

        public bool Alt { get; set; } = false;

        private IEnumerable<KeyboardKey> keyboardKeys
        {
            get
            {
                IList<string> keys = [];
                var resourceLoader = new ResourceLoader();

                if (Ctrl)
                {
                    keys.Add(resourceLoader.GetString("Settings_KeyboardShortcut_Ctrl"));
                }

                if (Shift)
                {
                    keys.Add(resourceLoader.GetString("Settings_KeyboardShortcut_Shift"));
                }

                if (Alt)
                {
                    keys.Add(resourceLoader.GetString("Settings_KeyboardShortcut_Alt"));
                }

                keys.Add(Key);
                return keys.Select(key => new KeyboardKey(key));
            }
        }

        public KeyboardShortcut() => InitializeComponent();
    }

    public sealed class KeyboardKey(string value)
    {
        public readonly string Value = value;
    }
}
