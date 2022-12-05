using System;
using System.Globalization;
using System.Windows.Data;
using HLab.Localization.Wpf.Lang;

namespace HLab.Mvvm
{
    public class LocalizationConverter : IValueConverter
    {
        static object LocalizeObject(object obj)
        {
            if (obj is string s)
            {
                return new Localize() {Id = s};
            }

            return obj;
        }


        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {

            return LocalizeObject(value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
