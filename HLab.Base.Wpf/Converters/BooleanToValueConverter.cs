using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HLab.Base.Converters
{
    public class BooleanToStringConverter : BooleanToValueConverter<string> { }
    public class BooleanToBrushConverter : BooleanToValueConverter<Brush> { }
    public class BooleanToVisibilityConverter : BooleanToValueConverter<Visibility> { }
    public class BooleanToObjectConverter : BooleanToValueConverter<object> { }
    public class BooleanToBooleanConverter : BooleanToValueConverter<bool> { }

    public class BooleanToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }
        public T NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? TrueValue : FalseValue;
            }

            return NullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
        {
            return value?.Equals(TrueValue) ?? false;
        }
    }
}