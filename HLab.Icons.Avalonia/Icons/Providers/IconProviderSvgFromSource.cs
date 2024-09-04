namespace HLab.Icons.Avalonia.Icons.Providers;

public class IconProviderSvgFromSource(string source, string name, int? foreColor) : IconProviderXamlParser
{
    protected override async Task<object?> ParseIconAsync() => await XamlTools.FromSvgStringAsync(source);

    protected override object? ParseIcon()=> XamlTools.FromSvgString(source);

}