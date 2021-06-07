using System.Windows.Media;

namespace HLab.ColorTools.Wpf
{
    /// <summary>
    /// RGB structure.
    /// </summary>
    public static class ErpColor
    {
        public static readonly Color[] DefaultColors =
        {
            System.Windows.Media.Colors.Blue,
            System.Windows.Media.Colors.DarkGreen,
            System.Windows.Media.Colors.DarkCyan,
            System.Windows.Media.Colors.DarkMagenta,
            System.Windows.Media.Colors.DarkRed,
            System.Windows.Media.Colors.DarkGoldenrod,
            System.Windows.Media.Colors.DarkKhaki
        };

        public static Color GetColor(int idx)
        {
            return DefaultColors[idx%DefaultColors.Length];
        }

        public static int ToInt(this Color c)
        {
            return (int)(((uint)c.A << 24) + ((uint)c.R << 16) + ((uint)c.G << 8) + (uint)c.B);
        }

        public static Color ToColor(this int? v)
        {
            return(v ?? 0).ToColor();
        }

        public static Color ToColor(this int v)
        {
            Color c = new Color
            {
                A = (byte) (((uint)v>>24) & 0xFF),
                R = (byte) (((uint)v>>16) & 0xFF),
                G = (byte) (((uint)v>>8) & 0xFF),
                B = (byte) ((uint)v & 0xFF),
            };
            return c;
        }

        public static HSL ToHSL(this Color c)
        {
            double h = 0, s = 0, l = 0;

            // normalize _r, _g, _b values
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max;
            double min;

            // hue
            if (r >= g)
            {
                if (r >= b)
                {
                    max = r;
                    if (g >= b) // r > g > b
                    {
                        min = b;
                        if (b >= r)
                        {
                            h = 0; // undefined
                        }
                        else
                        {
                            h = 60.0*(g - b)/(r - b);
                        }
                    }
                    else // r > b > g 
                    {
                        min = g;
                        h = 60.0*(g - b)/(r - g) + 360.0;
                    }
                }
                else // b > r > g
                {
                    max = b;
                    min = g;
                    h = 60.0*(r - g)/(b - g) + 240.0;
                }
            }
            else
            {
                if (g >= b) // g > r 
                {
                    max = g;
                    if (r >= b) // g > r > b
                    {
                        min = b;
                        h = 60.0*(b - r)/(g - b) + 120.0;
                    }
                    else // g > b > r
                    {
                        min = r;
                        h = 60.0*(b - r)/(g - r) + 120.0;
                    }
                }
                else // b > g > r
                {
                    max = b;
                    min = r;
                    h = 60.0 * (r - g) / (b - r) + 240.0;
                }
            }
            

            // luminance
            l = (max + min) / 2.0;

            // saturation
            if (l == 0 || max == min)
            {
                s = 0;
            }
            else if (0 < l && l <= 0.5)
            {
                s = (max - min) / (max + min);
            }
            else if (l > 0.5)
            {
                s = (max - min) / (2 - (max + min)); //(max-min > 0)?
            }

            return new HSL(h,s,l);
        }
    }
}
