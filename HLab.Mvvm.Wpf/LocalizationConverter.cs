using System;
using System.Globalization;
using System.Windows.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    public class LocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if(value is ILocalizationService l)
            {
                return l.LocalizeAsync((string)parameter);
            }
            if (parameter is string s)
                return s;

            return "_localization_failed_";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
