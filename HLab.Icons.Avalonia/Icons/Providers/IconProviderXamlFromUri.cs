using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Avalonia.Icons.Providers;

public class IconProviderXamlFromUri : IconProviderXamlParser, IIconProvider
{
    readonly Uri _uri;

    public IconProviderXamlFromUri(Uri uri)
    {
        _uri = uri;
    }
    protected override object? ParseIcon()
    {
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        return AvaloniaXamlLoader.Load(_uri);
    }
        
    protected override async Task<object?> ParseIconAsync()
    {
        object? icon = null;
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        await Dispatcher.UIThread.InvokeAsync(
            () => icon = AvaloniaXamlLoader.Load(_uri)
            ,XamlTools.Priority
        );

        return icon;
    }
}