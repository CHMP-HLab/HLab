using System;
using System.Globalization;
using System.Windows.Data;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if(value is IIconService l)
            {
                return l.GetIconAsync((string)parameter);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
