using System.Numerics;

namespace HLab.ColorTools;

public static partial class HLabColors
{
    public static ColorHSL<T> HSL<T>(T alpha, T hue, T saturation, T lightness) where T : INumber<T> 
        => new ColorHSL<T>(alpha, hue, saturation, lightness);

    public static ColorHSL<T> HSL<T>(T hue, T saturation, T lightness) where T : INumber<T> 
        => new ColorHSL<T>(ColorConst<T>.N, hue, saturation, lightness);
}


/// <summary>
/// Structure to define HSL.
/// </summary>
public readonly struct ColorHSL<T>(T alpha, T hue, T saturation, T lightness) : IColor<T> where T : INumber<T>
{
    /// <summary>
    /// Gets or sets the luminance component.
    /// </summary>
    public T Alpha { get; } = alpha.Cut(T.Zero, ColorConst<T>.N);

    /// <summary>
    /// Gets or sets the hue component.
    /// </summary>
    public T Hue { get; } = hue.Cut(T.Zero, ColorConst<T>.T360);

    /// <summary>
    /// Gets or sets saturation component.
    /// </summary>
    public T Saturation { get; } = saturation.Cut(T.Zero, ColorConst<T>.N);

    /// <summary>
    /// Gets or sets the lightness component.
    /// </summary>
    public T Lightness { get; } = lightness.Cut(T.Zero, ColorConst<T>.N);

    public static bool operator ==(ColorHSL<T> c1, ColorHSL<T> c2) =>
        c1.Alpha == c2.Alpha
        && c1.Hue == c2.Hue
        && c1.Saturation == c2.Saturation
        && c1.Lightness == c2.Lightness;

    public static bool operator !=(ColorHSL<T> c1, ColorHSL<T> c2) => !(c1 == c2);


    /// <summary>
    /// Creates an instance of a HSL structure.
    /// </summary>
    /// <param name="hue">Hue value.</param>
    /// <param name="saturation">Saturation value.</param>
    /// <param name="lightness">Lightness value.</param>
    public ColorHSL(T hue, T saturation, T lightness) : this(T.One, hue, saturation, lightness) { }

    public ColorHSL<T> ToHsl() => this;

    public static ColorHSL<T> From<TFrom>(ColorHSL<TFrom> c)
    where TFrom : INumber<TFrom>
    => new(
        ColorConst<T>.Normalize(c.Alpha),
        ColorConst<T>.Normalize(c.Hue, ColorConst<TFrom>.T360, ColorConst<T>.T360),
        ColorConst<T>.Normalize(c.Saturation),
        ColorConst<T>.Normalize(c.Lightness)
    );

    public ColorRGB<T> ToRGB() => RGB_HSL_Extensions.ToRGB( this );

    public ColorHSL<TTo> To<TTo>()
        where TTo : INumber<TTo>
        => ColorHSL<TTo>.From(this);

    IColor<TTo> IColor<T>.To<TTo>() => To<TTo>();

    public override bool Equals(object? obj) => obj is ColorHSL<T> c && this == c;

    public override int GetHashCode() => HashCode.Combine(Alpha.GetHashCode(), Hue.GetHashCode(), Saturation.GetHashCode(), Lightness.GetHashCode());

}