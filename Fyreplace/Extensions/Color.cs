using Fyreplace.Services;

namespace Fyreplace.Extensions
{
    public static class ColorExtensions
    {
        public static Windows.UI.Color ToWindowsColor(this Color color) => new()
        {
            R = (byte)color.R,
            G = (byte)color.G,
            B = (byte)color.B,
            A = byte.MaxValue
        };
    }
}
