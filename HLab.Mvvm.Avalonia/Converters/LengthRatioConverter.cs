using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Mvvm.Avalonia.Converters;

using H = DependencyHelper<LengthRatioConverter>;

public class LengthRatioConverter : AvaloniaObject, IValueConverter
{
    public static readonly StyledProperty<Rect> PhysicalRectProperty = H
        .Property<Rect>()
        .Default(new Rect())
        .Register();

    public static readonly StyledProperty<Control?> FrameworkElementProperty = H
        .Property<Control?>()
        .Default(null)
        .Register();


    public Rect PhysicalRect //{ get; set; }
    {
        get => GetValue(PhysicalRectProperty);
        set => SetValue(PhysicalRectProperty, value);
    }

    public Control? Element //{ get; set; }
    {
        get => GetValue(FrameworkElementProperty);
        set => SetValue(FrameworkElementProperty, value);
    }

    double Ratio => Math.Min(
        Element?.Bounds.Width??0 / PhysicalRect.Width,
        Element?.Bounds.Height??0 / PhysicalRect.Height
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