using System.Numerics;

namespace HLab.ColorTools;

/// <summary>
/// Structure to define HSB.
/// </summary>
public struct ColorHSB<T>(T alpha, T hue, T saturation, T brightness) : IColor<T> where T : INumber<T>
{
    /// <summary>
    /// Gets or sets the hue component.
    /// </summary>
    public T Alpha { get; } = alpha;

    /// <summary>
    /// Gets or sets the hue component.
    /// </summary>
    public T Hue { get; } = hue;

    /// <summary>
    /// Gets or sets saturation component.
    /// </summary>
    public T Saturation { get; } = saturation.Cut(T.Zero, ColorConst<T>.N);

    /// <summary>
    /// Gets or sets the brightness component.
    /// </summary>
    public T Brightness { get; } = brightness.Cut(T.Zero, ColorConst<T>.N);

    public static ColorHSB<T> From<TFrom>(ColorHSB<TFrom> c)
    where TFrom : INumber<TFrom>
    => new(
        ColorConst<T>.Normalize(c.Alpha),
        ColorConst<T>.Normalize(c.Hue),
        ColorConst<T>.Normalize(c.Saturation),
        ColorConst<T>.Normalize(c.Brightness)
    );

    public ColorRGB<T> ToRGB()
    {
        throw new NotImplementedException();
    }

    public ColorHSB<TTo> To<TTo>()
        where TTo : INumber<TTo>
        => ColorHSB<TTo>.From(this);

    IColor<TTo> IColor<T>.To<TTo>() => To<TTo>();

    public static bool operator ==(ColorHSB<T> c1, ColorHSB<T> c2) =>
        c1.Hue == c2.Hue
        && c1.Saturation == c2.Saturation
        && c1.Brightness == c2.Brightness;

    public static bool operator !=(ColorHSB<T> c1, ColorHSB<T> c2) => !(c1 == c2);

    public override bool Equals(object? obj) => obj is ColorHSB<T> c && this == c;

    public readonly override int GetHashCode() => HashCode.Combine(Hue, Saturation, Brightness);

}