using System.Numerics;

namespace HLab.ColorTools;

public static partial class HLabColors
{
    public static ColorRGB<T> RGB<T>(T alpha, T red, T green, T blue) where T : INumber<T> => new ColorRGB<T>(alpha, red, green, blue);
    public static ColorRGB<T> RGB<T>(T red, T green, T blue) where T : INumber<T> => new ColorRGB<T>(ColorConst<T>.N, red, green, blue);
}

public readonly struct ColorRGB<T>(T alpha, T red, T green, T blue) : IColor<T> where T : INumber<T>
{
    public T Alpha { get; } = alpha;
    public T Red { get; } = red;
    public T Green { get; } = green;
    public T Blue { get; } = blue;

    public ColorRGB<T> ToRGB() => this;

    public static ColorRGB<T> From<TFrom>(ColorRGB<TFrom> c)
        where TFrom : INumber<TFrom>
        => new(
            ColorConst<T>.Normalize(c.Alpha),
            ColorConst<T>.Normalize(c.Red),
            ColorConst<T>.Normalize(c.Green),
            ColorConst<T>.Normalize(c.Blue)
        );

    public ColorRGB<TTo> To<TTo>()
        where TTo : INumber<TTo>
        => ColorRGB<TTo>.From(this);

    IColor<TTo> IColor<T>.To<TTo>() => To<TTo>();
}