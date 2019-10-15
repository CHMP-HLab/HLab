using System;
using System.Globalization;
using System.Windows.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Icons
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if(value is IIconService l)
            {
                return l.GetIcon((string)parameter);
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
