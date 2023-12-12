using Avalonia;
using Avalonia.Controls;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Localization.Avalonia.Lang;

using H = DependencyHelper<LocalizedLabel>;
public partial class LocalizedLabel : Label, INamed
{
    public LocalizedLabel()
    {
        InitializeComponent();
    }

    //[Content]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string> TextProperty =
        H.Property<string>()
            .OnChanged((e, a)=> e.Localize.Id = e.Text)
            .Register();
}
