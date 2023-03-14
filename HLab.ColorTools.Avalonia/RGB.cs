using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace HLab.ColorTools.Avalonia
{
    /// <summary>
    /// RGB structure.
    /// </summary>
    public static partial class ColorExtensions
    {
        public static readonly Color[] DefaultColors =
        {
            Colors.Blue,
            Colors.DarkGreen,
            Colors.DarkCyan,
            Colors.DarkMagenta,
            Colors.DarkRed,
            Colors.DarkGoldenrod,
            Colors.DarkKhaki
        };

        public static Color GetColor(int idx)
        {
            return DefaultColors[idx%DefaultColors.Length];
        }

        public static int ToInt(this Color c)
        {
            return (int)(
                ((uint)c.A) + 
                ((uint)c.R << 8) + 
                ((uint)c.G << 16) + 
                ((uint)c.B << 24)
                );
        }

        public static Color ToColor(this int? v)
        {
            return(v ?? 0).ToColor();
        }

        public static Color ToColor(this int v)
        {
            var c = Color.FromArgb(
            
                (byte) ((uint)v & 0xFF),
                (byte) (((uint)v>>8) & 0xFF),
                (byte) (((uint)v>>16) & 0xFF),
                (byte) (((uint)v>>24) & 0xFF)
            );
            return c;
        }

        public static Color ToColor(this ColorByte c) => Unsafe.As<ColorByte, Color>(ref c);

        public static ColorByte ToColorByte(this Color c) => Unsafe.As<Color, ColorByte>(ref c);

        public static Color ToColor(this ColorDouble c) => new ColorByte(c).ToColor();
        public static ColorDouble ToColorDouble(this Color c) => new(c.ToColorByte());
    }
}
