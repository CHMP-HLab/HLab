using System.Runtime.CompilerServices;

namespace HLab.ColorTools
{
    /// <summary>
    /// Structure to define HSL.
    /// </summary>
    public readonly struct Hsl
    {
        /// <summary>
        /// Gets an empty HSL structure;
        /// </summary>
        public static readonly Hsl Empty = new();

        public Hsl Saturate(double ratio) =>
            new(
                Alpha,
                Hue,
                Saturation + ((1 - Saturation) * ratio),
                Luminance
            );

        public Hsl Desaturate(double ratio) =>
            new(
                Alpha,
                Hue,
                Saturation * ratio,
                Luminance
            );

        public Hsl Highlight(double ratio) =>
            new(
                Alpha,
                Hue,
                Saturation,
                Luminance + ((1 - Luminance) * ratio)
            );

        public Hsl Darken(double ratio) =>
            new(
                Alpha,
                Hue,
                Saturation,
                Luminance * ratio
            );

        public Hsl Transparent(double ratio) =>
            new(
                Alpha * ratio,
                Hue,
                Saturation,
                Luminance
            );

        public Hsl Opaque(double ratio) =>
            new(
                Alpha + ((1 -Alpha) * ratio),
                Hue,
                Saturation,
                Luminance
            );

        public static bool operator ==(Hsl item1, Hsl item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Luminance == item2.Luminance
                );
        }

        public static bool operator !=(Hsl item1, Hsl item2)
        {
            return (
                item1.Hue != item2.Hue
                || item1.Saturation != item2.Saturation
                || item1.Luminance != item2.Luminance
                );
        }

        /// <summary>
        /// Gets or sets the luminance component.
        /// </summary>
        public double Alpha { get; }

        /// <summary>
        /// Gets or sets the hue component.
        /// </summary>
        public double Hue { get; }

        /// <summary>
        /// Gets or sets saturation component.
        /// </summary>
        public double Saturation { get; }

        /// <summary>
        /// Gets or sets the luminance component.
        /// </summary>
        public double Luminance { get; }

        /// <summary>
        /// Creates an instance of a HSL structure.
        /// </summary>
        /// <param name="h">Hue value.</param>
        /// <param name="s">Saturation value.</param>
        /// <param name="l">Lightness value.</param>
        public Hsl(double h, double s, double l):this(1,h,s,l) { }
        public Hsl(double a, double h, double s, double l)
        {
            Alpha = (a > 1) ? 1 : ((a < 0) ? 0 : a);
            Hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            Saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            Luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
        }

        public Hsl(ColorDouble c)
        {
            double h = 0, s = 0, l = 0;

            double max;
            double min;

            // hue
            if (c.R >= c.G)
            {
                if (c.R >= c.B)
                {
                    max = c.R;
                    if (c.G >= c.B) // c.R > c.G > c.B
                    {
                        min = c.B;
                        if (c.B >= c.R)
                        {
                            h = 0; // undefined
                        }
                        else
                        {
                            h = 60.0*(c.G - c.B)/(c.R - c.B);
                        }
                    }
                    else // c.R > c.B > c.G 
                    {
                        min = c.G;
                        h = 60.0*(c.G - c.B)/(c.R - c.G) + 360.0;
                    }
                }
                else // c.B > c.R > c.G
                {
                    max = c.B;
                    min = c.G;
                    h = 60.0*(c.R - c.G)/(c.B - c.G) + 240.0;
                }
            }
            else
            {
                if (c.G >= c.B) 
                {
                    max = c.G;
                    if (c.R >= c.B) 
                    {
                        min = c.B;
                        h = 60.0*(c.B - c.R)/(c.G - c.B) + 120.0;
                    }
                    else
                    {
                        min = c.R;
                        h = 60.0*(c.B - c.R)/(c.G - c.R) + 120.0;
                    }
                }
                else
                {
                    max = c.B;
                    min = c.R;
                    h = 60.0 * (c.R - c.G) / (c.B - c.R) + 240.0;
                }
            }
            
            // luminance
            l = (max + min) / 2.0;

            // saturation
            if (l == 0 || Math.Abs(max - min) < double.Epsilon)
            {
                s = 0;
            }
            else if (l is > 0 and <= 0.5)
            {
                s = (max - min) / (max + min);
            }
            else if (l > 0.5)
            {
                s = (max - min) / (2 - (max + min)); 
            }

            Hue = h;
            Saturation = s;
            Luminance = l;
        }


        public override bool Equals(object? obj)
        {
            return obj switch
            {
                Hsl hsl => this == hsl,
                ColorByte c => this == new Hsl(new ColorDouble(c)),
                ColorDouble c => this == new Hsl(c),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^
                Luminance.GetHashCode();
        }


        /// <summary>
        /// Converts HSL to RGB.
        /// </summary>
        public ColorDouble ToColor()
        {
            if (Saturation == 0)
            {
                // achromatic color (gray scale)
                return new ColorDouble(
                    Alpha,
                    Luminance,
                    Luminance,
                    Luminance
                    );
            }

            var q = (Luminance < 0.5) ? (Luminance * (1.0 + Saturation)) : (Luminance + Saturation - (Luminance * Saturation));
            var p = (2.0 * Luminance) - q;

            var hk = Hue / 360.0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            double ToRgb(double t)
            {
                double tx6;

                if (t < 0) t += 1.0;
                else if (t > 1) t -= 1.0;

                if ((tx6 = t * 6) < 1)
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

            var tr = ToRgb(hk + (1.0 / 3.0));
            var tg = ToRgb(hk);
            var tb = ToRgb(hk - (1.0 / 3.0));

            return new ColorDouble(Alpha,tr,tg,tb);
        }

    }
}
