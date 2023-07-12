using System.Numerics;
using System.Runtime.CompilerServices;

namespace HLab.ColorTools;

public static class ArgbHslNativeExtensions
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <returns></returns>
    public static ColorHSL<double> ToHslDouble(this ColorRGB<double> c)
    {
        double h, s = 0.0;

        double max;
        double min;

        // hue
        if (c.Red >= c.Green)
        {
            if (c.Red >= c.Blue)
            {
                max = c.Red;
                if (c.Green >= c.Blue) // c.R > c.G > c.B
                {
                    min = c.Blue;
                    if (c.Blue >= c.Red)
                    {
                        h = 0.0; // undefined
                    }
                    else
                    {
                        h = 60.0 * (c.Green - c.Blue)/(c.Red - c.Blue);
                    }
                }
                else // c.R > c.B > c.G 
                {
                    min = c.Green;
                    h = 60.0 * (c.Green - c.Blue)/(c.Red - c.Green) + 360.0;
                }
            }
            else // c.B > c.R > c.G
            {
                max = c.Blue;
                min = c.Green;
                h = 60.0 * (c.Red - c.Green)/(c.Blue - c.Green) + 240.0;
            }
        }
        else
        {
            if (c.Green >= c.Blue) 
            {
                max = c.Green;
                if (c.Red >= c.Blue) 
                {
                    min = c.Blue;
                    h = 60.0 * (c.Blue - c.Red)/(c.Green - c.Blue) + 120.0;
                }
                else
                {
                    min = c.Red;
                    h = 60.0 * (c.Blue - c.Red)/(c.Green - c.Red) + 120.0;
                }
            }
            else
            {
                max = c.Blue;
                min = c.Red;
                h = 60.0 * (c.Red - c.Green) / (c.Blue - c.Red) + 240.0;
            }
        }

        // luminance
        var l = (max + min) / 2.0;

        // saturation
        if (l == 0.0 || max == min)
        {
            s = 0.0;
        }
        else if (l > 0.0 && l <= 0.5)
        {
            s = (max - min) / (max + min);
        }
        else if (l > 0.5)
        {
            s = (max - min) / (2.0 - (max + min)); 
        }

        return new( c.Alpha,h,s,l );
    }

    public static ColorRGB<double> ToRgbDouble(this ColorHSL<double> hsl)
    {
        if (hsl.Saturation == 0.0)
        {
            // achromatic color (gray scale)
            return new ColorRGB<double>(
                hsl.Alpha,
                hsl.Lightness,
                hsl.Lightness,
                hsl.Lightness
            );
        }

        var q = (hsl.Lightness < 0.5) ? (hsl.Lightness * (1.0 + hsl.Saturation)) : (hsl.Lightness + hsl.Saturation - (hsl.Lightness * hsl.Saturation));
        var p = (2.0 * hsl.Lightness) - q;

        var hk = hsl.Hue / 360.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double ToRGB(double t)
        {
            double tx6;

            if (t < 0.0) t += 1.0;
            else if (t > 1.0) t -= 1.0;

            if ((tx6 = t * 6.0) < 1.0)
            {
                t = p + ((q - p) * tx6);
            }
            else if (t * 2.0 < 1.0) //(1.0/6.0)<=T[i] && T[i]<0.5
            {
                t = q;
            }
            else if (t * 3.0 < 2.0) // 0.5<=T[i] && T[i]<(2.0/3.0)
            {
                t = p + (q - p) * ((6.0 * 2.0 / 3.0) - tx6);
            }
            else t = p;

            return t;
        }

        var tr = ToRGB(hk + 1.0 / 3.0);
        var tg = ToRGB(hk);
        var tb = ToRGB(hk - 1.0 / 3.0);

        return HLabColors.RGB(hsl.Alpha,tr,tg,tb); 
    }
}

public static class HSLConst<T> where T : INumber<T>
{
}

public static class RGB_HSL_Extensions
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <returns></returns>
    public static ColorHSL<T> ToHSL<T>(this ColorRGB<T> c) where T : INumber<T>
    {

        T h, s = T.Zero;

        T max;
        T min;

        // hue
        if (c.Red >= c.Green)
        {
            if (c.Red >= c.Blue)
            {
                max = c.Red;
                if (c.Green >= c.Blue) // c.R > c.G > c.B
                {
                    min = c.Blue;
                    if (c.Blue >= c.Red)
                    {
                        h = T.Zero; // undefined
                    }
                    else
                    {
                        h = ColorConst<T>.T60*(c.Green - c.Blue)/(c.Red - c.Blue);
                    }
                }
                else // c.R > c.B > c.G 
                {
                    min = c.Green;
                    h = ColorConst<T>.T60*(c.Green - c.Blue)/(c.Red - c.Green) + ColorConst<T>.T360;
                }
            }
            else // c.B > c.R > c.G
            {
                max = c.Blue;
                min = c.Green;
                h = ColorConst<T>.T60*(c.Red - c.Green)/(c.Blue - c.Green) + ColorConst<T>.T240;
            }
        }
        else
        {
            if (c.Green >= c.Blue) 
            {
                max = c.Green;
                if (c.Red >= c.Blue) 
                {
                    min = c.Blue;
                    h = ColorConst<T>.T60*(c.Blue - c.Red)/(c.Green - c.Blue) + ColorConst<T>.T120;
                }
                else
                {
                    min = c.Red;
                    h = ColorConst<T>.T60*(c.Blue - c.Red)/(c.Green - c.Red) + ColorConst<T>.T120;
                }
            }
            else
            {
                max = c.Blue;
                min = c.Red;
                h = ColorConst<T>.T60 * (c.Red - c.Green) / (c.Blue - c.Red) + ColorConst<T>.T240;
            }
        }

        // luminance
        var l = (max + min) / ColorConst<T>.T2;

        // saturation
        if (l == T.Zero || max == min)
        {
            s = T.Zero;
        }
        else if (l > T.Zero && l <= ColorConst<T>.Ndiv2)
        {
            s = (max - min) / (max + min);
        }
        else if (l > ColorConst<T>.Ndiv2)
        {
            s = (max - min) / (ColorConst<T>.T2 - (max + min)); 
        }

        return new( c.Alpha,h,s,l );
    }

    public static ColorRGB<T> ToRGB<T>(this ColorHSL<T> hsl) where T : INumber<T>
    {
        if (hsl.Saturation == T.Zero)
        {
            // achromatic color (gray scale)
            return new ColorRGB<T>(
                hsl.Alpha,
                hsl.Lightness,
                hsl.Lightness,
                hsl.Lightness
            );
        }

        var q = (hsl.Lightness < ColorConst<T>.Ndiv2) 
            ? (hsl.Lightness * (ColorConst<T>.N + hsl.Saturation)) 
            : (hsl.Lightness + hsl.Saturation - (hsl.Lightness * hsl.Saturation));

        var p = (ColorConst<T>.T2 * hsl.Lightness) - q;

        var hk = hsl.Hue / ColorConst<T>.T360;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T ToRgb(T t)
        {
            T tx6;

            if (t < T.Zero) t += T.One;
            else if (t > T.One) t -= T.One;

            if ((tx6 = t * ColorConst<T>.T6) < T.One)
            {
                t = p + ((q - p) * tx6);
            }
            else if (t * ColorConst<T>.T2 < T.One) //(1.0/6.0)<=T[i] && T[i]<0.5
            {
                t = q;
            }
            else if (t * ColorConst<T>.T3 < ColorConst<T>.T2) // 0.5<=T[i] && T[i]<(2.0/3.0)
            {
                t = p + (q - p) * ( ColorConst<T>.T4 /*(6.0 * 2.0 / 3.0)*/ - tx6);
            }
            else t = p;

            return t;
        }

        var tr = ToRgb(hk + ColorConst<T>.Ndiv3);
        var tg = ToRgb(hk);
        var tb = ToRgb(hk - ColorConst<T>.Ndiv3);

        return new ColorRGB<T>(hsl.Alpha,tr,tg,tb);
    }



}