using System.Numerics;

namespace HLab.ColorTools;

internal static class NumberExtensions
{
    public static T Normalized<T>(this double value) where T : INumber<T> => T.CreateChecked(value) * ColorConst<T>.N;

    public static T N<T>(this double value) where T : INumber<T> => T.CreateChecked(value);
    public static T N<T>(this int value) where T : INumber<T> => T.CreateChecked(value);
    public static T N<T>(this float value) where T : INumber<T> => T.CreateChecked(value);
    public static T HighCut<T>(this T value, T max) where T : INumber<T> => (value<max)?value:max;
    public static T LowCut<T>(this T value, T min) where T : INumber<T> => (value<min)?min:value;
    public static T Cut<T>(this T value, T min, T max) where T : INumber<T> => (value<min)?min:(value<max)?value:max;

}

internal static class ColorConst<T> where T : INumber<T>
{

    static T GetN()
    {
        if(typeof(T) == typeof(byte)) return 255.N<T>();
        if(typeof(T) == typeof(double)) return 1.0.N<T>();
        if(typeof(T) == typeof(int)) return 0x00FF.N<T>();
        if(typeof(T) == typeof(float)) return 1.0f.N<T>();
        throw new NotImplementedException();
    }
    static T GetN360()
    {
        if(typeof(T) == typeof(byte)) return 255.N<T>();
        if(typeof(T) == typeof(double)) return 360.0.N<T>();
        if(typeof(T) == typeof(int)) return 0x00FF.N<T>();
        if(typeof(T) == typeof(float)) return 360.0f.N<T>();
        throw new NotImplementedException();
    }

    public static readonly T N = GetN();

    public static double NDouble = double.CreateSaturating(N);

    public static T Normalize<TFrom>(TFrom v, TFrom from, T to)
    where TFrom : INumber<TFrom>
    {
        var d = double.CreateSaturating(to);
        d *= double.CreateSaturating(v);
        d /= double.CreateSaturating(from);

        return T.CreateSaturating(d);
    }

    public static T Normalize<TFrom>(TFrom v)
    where TFrom : INumber<TFrom>
    {
        var d = NDouble;
        d *= double.CreateSaturating(v);
        d /= ColorConst<TFrom>.NDouble;

        return T.CreateSaturating(d);
    }

    public static readonly T T2  = 2.0.N<T>();
    public static readonly T T3  = 3.0.N<T>();

    public static readonly T Ndiv2 = N / T2;
    public static readonly T Ndiv3  = N / T3;

    public static readonly T T4  = 4.0.N<T>();
    public static readonly T T6  = 6.0.N<T>();

    public static readonly T T360 = GetN360();
    public static readonly T T120 = T360 / T3;
    public static readonly T T60  = T120 / T2;
    public static readonly T T240 = T120 * T2;
}