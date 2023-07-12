using System.Numerics;
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

        public static Color ToAvaloniaColor(this int? v)
        {
            return(v ?? 0).ToAvaloniaColor();
        }

        public static Color ToAvaloniaColor(this int v)
        {
            var c = Color.FromArgb(
            
                (byte) ((uint)v & 0xFF),
                (byte) (((uint)v>>8) & 0xFF),
                (byte) (((uint)v>>16) & 0xFF),
                (byte) (((uint)v>>24) & 0xFF)
            );
            return c;
        }

        public static Color ToAvaloniaColor(this uint v)
        {
            var c = Color.FromArgb(
            
                (byte) ((uint)v & 0xFF),
                (byte) (((uint)v>>8) & 0xFF),
                (byte) (((uint)v>>16) & 0xFF),
                (byte) (((uint)v>>24) & 0xFF)
            );
            return c;
        }

        public static Color ToAvaloniaColor(this IColor<byte> c)
        {
            var rgb = c.ToRGB();
            return Unsafe.As<ColorRGB<byte>, Color>(ref rgb);
        }

        public static ColorRGB<byte> ToColor(this Color c) => Unsafe.As<Color, ColorRGB<byte>>(ref c);

        public static Color ToAvaloniaColor<T>(this IColor<T> c) where T:INumber<T> => c.To<byte>().ToAvaloniaColor();
        public static ColorRGB<T> ToColor<T>(this Color c) where T:INumber<T>  => c.ToColor().To<T>();


    }
}
