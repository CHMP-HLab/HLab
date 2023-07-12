namespace HLab.Icons.Avalonia.Icons.Providers;

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