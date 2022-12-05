using System;
using System.Windows;
using System.Windows.Data;

namespace HLab.Base.Converters
{
    public class LengthRatioConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty PhysicalRectProperty =
            DependencyProperty.Register(nameof(PhysicalRect), typeof(Rect),
                typeof(LengthRatioConverter), new FrameworkPropertyMetadata(new Rect()));

        public static readonly DependencyProperty FrameworkElementProperty =
            DependencyProperty.Register(nameof(FrameworkElement), typeof(FrameworkElement),
                typeof(LengthRatioConverter), new FrameworkPropertyMetadata(null));

        public Rect PhysicalRect //{ get; set; }
        {
            get => (Rect)GetValue(PhysicalRectProperty);
            set => SetValue(PhysicalRectProperty, value);
        }

        public FrameworkElement FrameworkElement //{ get; set; }
        {
            get => (FrameworkElement)GetValue(FrameworkElementProperty);
            set => SetValue(FrameworkElementProperty, value);
        }

        double Ratio => Math.Min(
            FrameworkElement.ActualWidth / PhysicalRect.Width,
            FrameworkElement.ActualHeight / PhysicalRect.Height
        );

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new GridLength(Ratio * (double) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((GridLength)value).Value / Ratio;
        }

        //public override object ProvideValue(IServiceProvider serviceProvider)
        //{
        //    return this;
        //}
    }
}