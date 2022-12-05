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
        readonly string _source;
        readonly int? _foreColor;
 
        public IconProviderXamlFromSource(string source, string name, int? foreground)
        { 
            _source = source; 
            _name = name;
            _foreColor = foreground;
        }

        protected override async Task<object> GetActualAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            var icon = await XamlTools.FromXamlStringAsync(_source);

            //if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
            //    await XamlTools.SetForegroundAsync(o, _foreColor.ToColor(), brush);

            return icon;
        }
        public override async Task<string> GetTemplateAsync()
        {
            return _source;
        }

        public override object Get()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            var icon = XamlTools.FromXamlString(_source);

            //if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
            //    XamlTools.SetForeground(o, _foreColor.ToColor(), brush);

            return icon;
        }
    }
}