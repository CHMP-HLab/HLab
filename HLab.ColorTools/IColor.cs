using System.Numerics;

namespace HLab.ColorTools;

public interface IColor<T> where T : INumber<T>
{
    T Alpha { get; }

    ColorRGB<T> ToRGB();

    IColor<TTo> To<TTo>() where TTo : INumber<TTo>;
}


