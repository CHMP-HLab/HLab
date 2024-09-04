using System.Numerics;

namespace HLab.ColorTools;

public static class ColorCorrectionRgbExtentions
{
    public static ColorRGB<T> Multiply<T>(this ColorRGB<T> c, T value) where T : INumber<T>
        => new(c.Alpha, c.Red * value, c.Green * value, c.Blue * value);

}

public static class ColorCorrectionHslExtensions
{

    public static T More<T>(this T value, T ratio)
        where T : INumber<T>
        => value + T.CreateSaturating(
                double.CreateSaturating(ColorConst<T>.N - value)
                * double.CreateSaturating(ratio / ColorConst<T>.N)
                );

    public static T Less<T>(this T value, T ratio)
        where T : INumber<T>
        => T.CreateSaturating(
                double.CreateSaturating(value)
                * double.CreateSaturating(ratio / ColorConst<T>.N)
                );


    public static ColorHSL<T> WithAlpha<T>(this ColorHSL<T> color, T alpha)
    where T : INumber<T> => HLabColors.HSL
    (
        alpha,
        color.Hue,
        color.Saturation,
        color.Lightness
    );

    public static ColorHSL<T> WithHue<T>(this ColorHSL<T> color, T hue)
    where T : INumber<T> => HLabColors.HSL
    (
        color.Alpha,
        hue,
        color.Saturation,
        color.Lightness
    );

    public static ColorHSL<T> WithLightness<T>(this ColorHSL<T> color, T lightness)
    where T : INumber<T> => HLabColors.HSL
    (
        color.Alpha,
        color.Hue,
        color.Saturation,
        lightness
    );

    public static ColorHSL<T> WithSaturation<T>(this ColorHSL<T> color, T saturation)
    where T : INumber<T> => HLabColors.HSL
    (
        color.Alpha,
        color.Hue,
        saturation,
        color.Lightness
    );

    public static ColorHSL<T> LessOpacity<T>(this ColorHSL<T> color, T ratio)
        where T : INumber<T>
        => color.WithAlpha(color.Alpha.Less(ratio));

    public static ColorHSL<T> MoreOpacity<T>(this ColorHSL<T> color, T ratio)
        where T : INumber<T>
        => color.WithAlpha(color.Alpha.More(ratio));


    public static ColorHSL<T> Saturate<T>(this ColorHSL<T> color, T ratio)
        where T : INumber<T> 
        => color.WithSaturation(color.Saturation.More(ratio));

    public static ColorHSL<T> Desaturate<T>(this ColorHSL<T> color, T ratio)
        where T : INumber<T> 
        => color.WithSaturation(color.Saturation.Less(ratio));

    public static ColorHSL<T> Highlight<T>(this ColorHSL<T> color, T ratio)
        where T : INumber<T> 
        => color.WithLightness(color.Lightness.More(ratio));

    public static ColorHSL<T> Darken<T>(this ColorHSL<T> color, T ratio)
        where T : INumber<T>
        => color.WithLightness(color.Lightness.Less(ratio));

}