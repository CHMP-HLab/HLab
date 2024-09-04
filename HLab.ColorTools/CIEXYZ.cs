using System.Numerics;

namespace HLab.ColorTools;

/// <summary>
/// Structure to define CIE XYZ.
/// </summary>
public readonly struct CIEXYZ<T> where T : INumber<T>
{
    /// <summary>
    /// Gets an empty CIEXYZ structure.
    /// </summary>
    public static readonly CIEXYZ<T> Empty = new();

    /// <summary>
    /// Gets the CIE D65 (white) structure.
    /// </summary>
    public static readonly CIEXYZ<T> D65 = new(_0_9505, T.One, _1_089);


    public static bool operator ==(CIEXYZ<T> item1, CIEXYZ<T> item2) =>
    item1.X == item2.X
    && item1.Y == item2.Y
    && item1.Z == item2.Z;

    public static bool operator !=(CIEXYZ<T> item1, CIEXYZ<T> item2) =>
    item1.X != item2.X
    || item1.Y != item2.Y
    || item1.Z != item2.Z;

    /// <summary>
    /// Gets or sets X component.
    /// </summary>
    public T X { get; }

    /// <summary>
    /// Gets or sets Y component.
    /// </summary>
    public T Y { get; }

    /// <summary>
    /// Gets or sets Z component.
    /// </summary>
    public T Z { get; }

    static readonly T _0_9505 = 0.9505.N<T>();
    static readonly T _1_089 = 1.089.N<T>();

    public CIEXYZ(T x, T y, T z)
    {
        X = x.Cut(T.Zero,_0_9505);
        Y = y.Cut(T.Zero, T.One);
        Z = z.Cut(T.Zero,_1_089);

        _hashcode = HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CIEXYZ<T> ciexyz) return false;
        return this == ciexyz;
    }

    readonly int _hashcode;
    public override int GetHashCode() => _hashcode;
}