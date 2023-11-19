using Avalonia;
using Avalonia.Controls;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Mvvm.Avalonia;

using H = DependencyHelper<DefaultWindow>;

/// <summary>
/// Logique d'interaction pour DefaultWindow.xaml
/// </summary>
public partial class DefaultWindow : Window
{
    public DefaultWindow()
    {
        InitializeComponent();

    }
    public object? View
    {
        get => GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    public static readonly StyledProperty<object?> ViewProperty =
        H.Property<object?>()
            .OnChanged((w,e) =>
            {
                w.ContentControl.Content = e.NewValue.Value;
            })
            .Register();


}
