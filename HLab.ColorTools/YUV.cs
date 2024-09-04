using System.Numerics;

namespace HLab.ColorTools;

/// <summary>
/// Structure to define YUV.
/// </summary>
public readonly struct YUV<T>(T alpha, T y, T u, T v) where T : INumber<T>
{
    public T Alpha { get; } = alpha;

    public T Y { get; } = y.Cut(T.Zero, ColorConst<T>.N);

    public T U { get; } = u.Cut(-_0_436, _0_436);

    public T V { get; } = v.Cut(-_0_615, _0_615);

    static T _0_436 = 0.436.Normalized<T>();
    static T _0_615 = 0.615.Normalized<T>();

    public static bool operator ==(YUV<T> c1, YUV<T> c2) 
        => c1.Y == c2.Y 
           && c1.U == c2.U
           && c1.V == c2.V;

    public static bool operator != (YUV<T> item1, YUV<T> item2) => !(item1 == item2);


    ///// <summary>
    ///// Creates an instance of a YUV structure.
    ///// </summary>
    //public YUV(double y, double u, double v)
    //{
    //    Y = (y > 1) ? 1 : ((y < 0) ? 0 : y);
    //    U = (u > 0.436) ? 0.436 : ((u < -0.436) ? -0.436 : u);
    //    V = (v > 0.615) ? 0.615 : ((v < -0.615) ? -0.615 : v);
    //}

    public override bool Equals(object? obj)
    {
        if (obj is YUV<T> yuv) return this == yuv;
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Y, U, V);

}