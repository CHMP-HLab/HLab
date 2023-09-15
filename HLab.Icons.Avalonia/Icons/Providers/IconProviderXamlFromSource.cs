using HLab.Mvvm.Annotations;

namespace HLab.Icons.Avalonia.Icons.Providers;

public class IconProviderXamlFromSource : IconProviderXaml, IIconProvider
{
    readonly string _name;
    readonly int? _foreColor;
 
    public IconProviderXamlFromSource(string source, string name, int? foreground):base(source)
    { 
        _name = name;
        _foreColor = foreground;
    }
}