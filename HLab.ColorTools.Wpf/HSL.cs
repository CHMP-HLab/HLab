using System;
using System.Windows.Media;

namespace HLab.ColorTools.Wpf
{
    /// <summary>
    /// Structure to define HSL.
    /// </summary>
    public struct HSL
    {
        /// <summary>
        /// Gets an empty HSL structure;
        /// </summary>
        public static readonly HSL Empty = new HSL();

        double hue;
        double saturation;
        double luminance;
        double alpha;

        public HSL Saturate(double ratio)
        {
            HSL hsl = this;
            hsl.Saturation += (1 - hsl.Saturation) * ratio;
            return hsl;
        }

        public HSL Desaturate(double ratio)
        {
            HSL hsl = this;
            hsl.Saturation *= ratio;
            return hsl;
        }

        public HSL Highlight(double ratio)
        {
            HSL hsl = this;
            hsl.Luminance += (1 - hsl.Luminance) * ratio;
            return hsl;
        }

        public HSL Darken(double ratio)
        {
            HSL hsl = this;
            hsl.Luminance *= ratio;
            return hsl;
        }
        public HSL Transparent(double ratio)
        {
            HSL hsl = this;
            hsl.Alpha *= ratio;
            return hsl;
        }
        public HSL Opaque(double ratio)
        {
            HSL hsl = this;
            hsl.Alpha += (1 - hsl.Alpha) * ratio;
            return hsl;
        }

        public static bool operator ==(HSL item1, HSL item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Luminance == item2.Luminance
                );
        }

        public static bool operator !=(HSL item1, HSL item2)
        {
            return (
                item1.Hue != item2.Hue
                || item1.Saturation != item2.Saturation
                || item1.Luminance != item2.Luminance
                );
        }

        /// <summary>
        /// Gets or sets the hue component.
        /// </summary>
        public double Hue
        {
            get => hue; set => hue = (value > 360) ? 360 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Gets or sets saturation component.
        /// </summary>
        public double Saturation
        {
            get => saturation; set => saturation = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Gets or sets the luminance component.
        /// </summary>
        public double Luminance
        {
            get => luminance; set => luminance = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }
        /// <summary>
        /// Gets or sets the luminance component.
        /// </summary>
        public double Alpha
        {
            get => alpha; set => alpha = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Creates an instance of a HSL structure.
        /// </summary>
        /// <param name="h">Hue value.</param>
        /// <param name="s">Saturation value.</param>
        /// <param name="l">Lightness value.</param>
        public HSL(double h, double s, double l)
        {
            this.hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            this.saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            this.luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
            this.alpha = 1;
        }
        public HSL(double a, double h, double s, double l)
        {
            this.hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            this.saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            this.luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
            this.alpha = (a > 1) ? 1 : ((a < 0) ? 0 : a);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (HSL)obj);
        }

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^
                Luminance.GetHashCode();
        }

        public static implicit operator Color(HSL hsl)
        {
            return hsl.ToColor();
        }

        /// <summary>
        /// Converts HSL to RGB.
        /// </summary>
        public Color ToColor()
        {
            if (saturation == 0)
            {
                // achromatic color (gray scale)
                return Color.FromArgb(
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        alpha * 255.0))),
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        luminance * 255.0))),
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        luminance * 255.0))),
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        luminance * 255.0)))
                    );
            }
            else
            {
                double q = (luminance < 0.5) ? (luminance * (1.0 + saturation)) : (luminance + saturation - (luminance * saturation));
                double p = (2.0 * luminance) - q;

                double Hk = hue / 360.0;
                double[] T = new double[3];
                T[0] = Hk + (1.0 / 3.0);    // Tr
                T[1] = Hk;                // Tb
                T[2] = Hk - (1.0 / 3.0);    // Tg

                for (int i = 0; i < 3; i++)
                {
                    if (T[i] < 0) T[i] += 1.0;
                    if (T[i] > 1) T[i] -= 1.0;

                    if ((T[i] * 6) < 1)
                    {
                        T[i] = p + ((q - p) * 6.0 * T[i]);
                    }
                    else if ((T[i] * 2.0) < 1) //(1.0/6.0)<=T[i] && T[i]<0.5
                    {
                        T[i] = q;
                    }
                    else if ((T[i] * 3.0) < 2) // 0.5<=T[i] && T[i]<(2.0/3.0)
                    {
                        T[i] = p + (q - p) * ((2.0 / 3.0) - T[i]) * 6.0;
                    }
                    else T[i] = p;
                }

                return Color.FromArgb(
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        alpha * 255.0))),
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        T[0] * 255.0))),
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        T[1] * 255.0))),
                    Convert.ToByte(Double.Parse(String.Format("{0:0.00}",
                        T[2] * 255.0)))
                    );
            }
        }

    }
}
