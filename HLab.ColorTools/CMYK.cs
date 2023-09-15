using System.Numerics;

namespace HLab.ColorTools;

/// <summary>
/// Structure to define CMYK.
/// </summary>
public readonly struct ColorCMYK<T>(T alpha, T cyan, T magenta, T yellow, T black) : IColor<T> where T : INumber<T>
{
    public T Alpha { get; } = alpha.Cut(T.Zero,ColorConst<T>.N);

    public T Cyan { get; } = cyan.Cut(T.Zero,ColorConst<T>.N);

    public T Magenta { get; } = magenta.Cut(T.Zero,ColorConst<T>.N);

    public T Yellow { get; } = yellow.Cut(T.Zero,ColorConst<T>.N);

    public T Black { get; } = black.Cut(T.Zero,ColorConst<T>.N);

    public ColorRGB<T> ToRGB()
    {
        throw new NotImplementedException();
    }

    public static ColorCMYK<T> From<TFrom>(ColorCMYK<TFrom> c)
        where TFrom : INumber<TFrom>
        => new(
            ColorConst<T>.Normalize(c.Alpha),
            ColorConst<T>.Normalize(c.Cyan),
            ColorConst<T>.Normalize(c.Magenta),
            ColorConst<T>.Normalize(c.Yellow),
            ColorConst<T>.Normalize(c.Black)
        );

    public ColorCMYK<TTo> To<TTo>()
        where TTo : INumber<TTo>
        => ColorCMYK<TTo>.From(this);

    IColor<TTo> IColor<T>.To<TTo>() => To<TTo>();

    public static bool operator ==(ColorCMYK<T> c1, ColorCMYK<T> c2) =>
        c1.Cyan == c2.Cyan
        && c1.Magenta == c2.Magenta
        && c1.Yellow == c2.Yellow
        && c1.Black == c2.Black;

    public static bool operator !=(ColorCMYK<T> c1, ColorCMYK<T> c2) => !(c1 == c2);

    public override bool Equals(object? obj)
    {
        if (obj is not ColorCMYK<T> c) return false;
        return this == c;
    }

    public override int GetHashCode() => HashCode.Combine(Cyan.GetHashCode(), Magenta.GetHashCode(), Yellow.GetHashCode(), Black.GetHashCode());

}