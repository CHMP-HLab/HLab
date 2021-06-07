using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using HLab.ColorTools.Wpf;

namespace HLab.Mvvm.Flowchart.Views
{
    public class ColorToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush
                {
                    Color = color.AdjustBrightness(0.5)
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class ColorToGradientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                var c = color.AdjustBrightness(-0.5);

                return new LinearGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(color,0),
                        new GradientStop(Color.FromScRgb(0.5f,c.ScR,c.ScG,c.ScB),1)
                    }
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
