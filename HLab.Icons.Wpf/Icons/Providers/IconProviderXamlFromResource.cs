using System.Resources;
using System.Threading.Tasks;
using HLab.Icons.Annotations.Icons;
using Color = System.Windows.Media.Color;

namespace HLab.Icons.Wpf.Icons.Providers;

public class IconProviderXamlFromResource : IconProviderXamlParser, IIconProvider
{
    readonly ResourceManager _resourceManager;
    readonly string _name;
    readonly Color? _foreColor;
 
    public IconProviderXamlFromResource(ResourceManager resourceManager, string name, Color? foreColor)
    { 
        _resourceManager = resourceManager; 
        _name = name;
        _foreColor = foreColor;
    }

    protected override object? ParseIcon()
    {
        using var xamlStream = _resourceManager.GetStream(_name);
        return xamlStream is null ? null : XamlTools.FromXamlStream(xamlStream);
    }

    protected override async Task<object?> ParseIconAsync()
    {
        await using var xamlStream = _resourceManager.GetStream(_name);
        if (xamlStream == null) return null;
        return await XamlTools.FromXamlStreamAsync(xamlStream).ConfigureAwait(true);
    }
}