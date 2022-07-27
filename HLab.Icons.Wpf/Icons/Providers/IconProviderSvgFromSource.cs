using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderSvgFromSource : IconProvider, IIconProvider
    {
        readonly string _name;
        string _source;
        readonly int? _foreColor;
        bool _parsed = false;

        public IconProviderSvgFromSource(string source, string name, int? foreColor)
        {
            _foreColor = foreColor;
            _source = source; 
            _name = name;
        }
        protected override async Task<object> GetAsync(object foreground = null)
        {
            if (string.IsNullOrWhiteSpace(_source)) return null;

            object icon;
            if (_parsed)
            {
                icon = await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(true);
            }
            else
            {
                icon = await XamlTools.FromSvgStringAsync(_source).ConfigureAwait(true);
                _source = XamlWriter.Save(icon);
                _parsed = true;
            }

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.ToColor(), brush);

            return icon;
        }

        protected override object Get(object foreground = null)
        {
            if (string.IsNullOrWhiteSpace(_source)) return null;

            object icon;
            if (_parsed)
            {
                icon = XamlTools.FromXamlString(_source);
            }
            else
            {
                icon = XamlTools.FromSvgString(_source);
                _source = XamlWriter.Save(icon);
                _parsed = true;
            }

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.ToColor(), brush);

            return icon;
        }
    }
}