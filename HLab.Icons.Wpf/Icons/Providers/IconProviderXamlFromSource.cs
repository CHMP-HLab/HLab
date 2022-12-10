using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers;

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