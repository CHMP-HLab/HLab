using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromSource : IconProvider, IIconProvider
    {
        readonly string _name;
        string _source;
        readonly int? _foreColor;
        bool _parsed = false;
 
        public IconProviderXamlFromSource(string source, string name, int? foreground)
        { 
            _source = source; 
            _name = name;
            _foreColor = foreground;
        }

        protected override async Task<object> GetAsync(object foreground = null)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            var icon = await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(true);

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.ToColor(), brush);

            return icon;
        }

        protected override object Get(object foreground = null)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            var icon = XamlTools.FromXamlString(_source);

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.ToColor(), brush);

            return icon;
        }
    }
}