using Fyreplace.Services;

namespace Fyreplace.Extensions
{
    public static class ColorExtensions
    {
        public static Windows.UI.Color ToWindowsColor(this Color self) => new()
        {
            R = (byte)self.R,
            G = (byte)self.G,
            B = (byte)self.B,
            A = byte.MaxValue
        };
    }
}
