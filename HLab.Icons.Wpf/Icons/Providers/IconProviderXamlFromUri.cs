using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers;

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
        return Application.LoadComponent(_uri);;
    }
        
    protected override async Task<object?> ParseIconAsync()
    {
        object? icon = null;
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        await Application.Current.Dispatcher.InvokeAsync(
            () => icon = Application.LoadComponent(_uri)
            ,XamlTools.Priority
        );

        return icon;
    }
}