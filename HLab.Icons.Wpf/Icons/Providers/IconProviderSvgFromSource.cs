using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers;

public class IconProviderSvgFromSource : IconProviderXamlParser, IIconProvider
{
    readonly string _name;
    readonly string _source;
 
    public IconProviderSvgFromSource(string source, string name, int? foreColor)
    {
        _source = source; 
        _name = name;
    }

    protected override async Task<object?> ParseIconAsync() => await XamlTools.FromSvgStringAsync(_source);

    protected override object? ParseIcon()=> XamlTools.FromSvgString(_source);

}