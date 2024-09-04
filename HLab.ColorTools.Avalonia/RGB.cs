using System.Numerics;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace HLab.ColorTools.Avalonia;

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
    public static Color ToColor(this IBrush brush) =>
    (
        brush switch
        {
            ISolidColorBrush s => s.Color,
            IGradientBrush g => g.GradientStops.AverageColor(),
            //ConicGradientBrush cg => cg.GradientStops.Average(),
            _ => Colors.Red
        }
    );

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


        
    public readonly struct WeightedColor
    {

        public double W { get; init; }
        public double A { get; init; }
        public double R { get; init; }
        public double G { get; init; }
        public double B { get; init; }

        public static WeightedColor FromColor(Color c, double w) => new()
        {
            A = c.A / 255.0,
            R = c.R / 255.0,
            G = c.G / 255.0,
            B = c.B / 255.0,
            W = w
        };

        public Color Color => new((byte)(A * 255.0), (byte)(R * 255.0), (byte)(G * 255.0), (byte)(B * 255.0));

        public static WeightedColor Average(params WeightedColor[] color)
        {
            var w = 0.0;

            var a = 0.0;

            var r = 0.0;
            var g = 0.0;
            var b = 0.0;

            foreach (var wc in color)
            {
                w += wc.W;
                a += wc.A * wc.W;
                r += wc.R * wc.W;
                g += wc.G * wc.W;
                b += wc.B * wc.W;
            }

            return new WeightedColor
            {
                A = a / w,
                R = r / w,
                G = g / w,
                B = b / w,
                W = w
            };

        }
    }



    public static Color AverageColor(this IEnumerable<IGradientStop> @this)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        var stops = @this.ToArray();

        double w;

        var color = stops[0].Color;
        var offset = 0.0;

        //            Span<WeightedColor> wc = stackalloc WeightedColor[@this.Count+1];
        var wc = new WeightedColor[stops.Length + 1];

        var i = 0;
        while (i < stops.Length)
        {
            w = (stops[i].Offset - offset) / 2;

            wc[i] = WeightedColor.Average(
                WeightedColor.FromColor(color, w),
                WeightedColor.FromColor(stops[i].Color, w)
            );

            color = stops[i].Color;
            offset = stops[i].Offset;
            i++;
        }

        w = (1.0 - offset) / 2;

        wc[i] = WeightedColor.Average(
            WeightedColor.FromColor(color, w),
            WeightedColor.FromColor(stops[i].Color, w)
        );

        return WeightedColor.Average(wc).Color;
    }


}