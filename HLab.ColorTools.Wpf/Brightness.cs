using System.Windows.Media;

namespace HLab.ColorTools.Wpf
{
    public static class ColorExtensions
    {
        public static Color AdjustBrightness(this Color color, double correctionFactor)
        {
            var red = (double)color.R;
            var green = (double)color.G;
            var blue = (double)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}
