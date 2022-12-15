using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HLab.Base.Wpf.Converters
{
    public class ToStringConverter : ToValueConverter<string> { }
    public class ToBrushConverter : ToValueConverter<Brush> { }
    public class ToVisibilityConverter : ToValueConverter<Visibility> 
    {
        public ToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
            NullValue = Visibility.Collapsed;
        }
    }
    public class ToObjectConverter : ToValueConverter<object> { }
    public class ToBooleanConverter : ToValueConverter<bool> { }



    public class ToValueConverter<T> : IValueConverter
    {
        public T NullValue { get; set; }
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? TrueValue : FalseValue;
            }
            if (value is string s)
            {
                return string.IsNullOrWhiteSpace(s) ? FalseValue : TrueValue;
            }
            if (value is int i)
            {
                return i==0 ? FalseValue : TrueValue;
            }

            return NullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}