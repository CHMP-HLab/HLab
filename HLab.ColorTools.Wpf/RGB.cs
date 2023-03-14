using System.Windows.Media;

namespace HLab.ColorTools.Wpf
{
    /// <summary>
    /// RGB structure.
    /// </summary>
    public static class ErpColor
    {
        public static readonly Color[] DefaultColors =
        {
            System.Windows.Media.Colors.Blue,
            System.Windows.Media.Colors.DarkGreen,
            System.Windows.Media.Colors.DarkCyan,
            System.Windows.Media.Colors.DarkMagenta,
            System.Windows.Media.Colors.DarkRed,
            System.Windows.Media.Colors.DarkGoldenrod,
            System.Windows.Media.Colors.DarkKhaki
        };

        public static Color GetColor(int idx)
        {
            return DefaultColors[idx%DefaultColors.Length];
        }

        public static int ToInt(this Color c)
        {
            return (int)(((uint)c.A << 24) + ((uint)c.R << 16) + ((uint)c.G << 8) + (uint)c.B);
        }

        public static Color ToColor(this int? v)
        {
            return(v ?? 0).ToColor();
        }

        public static Color ToColor(this int v)
        {
            var c = new Color
            {
                A = (byte) (((uint)v>>24) & 0xFF),
                R = (byte) (((uint)v>>16) & 0xFF),
                G = (byte) (((uint)v>>8) & 0xFF),
                B = (byte) ((uint)v & 0xFF),
            };
            return c;
        }
    }
}
