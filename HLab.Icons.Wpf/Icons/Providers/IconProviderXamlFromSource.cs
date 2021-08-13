using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromSource : IconProvider, IIconProvider
    {
        private readonly string _name;
        private string _source;
        private readonly int? _foreground;
        private bool _parsed = false;
 
        public IconProviderXamlFromSource(string source, string name, int? foreground)
        { 
            _source = source; 
            _name = name;
            _foreground = foreground;
        }

        protected override async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            var icon = await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(true);

            if (!_parsed && _foreground != null && icon is UIElement ui)
            {
                XamlTools.SetBinding(ui, _foreground.ToColor());
                _source = XamlWriter.Save(ui);
                _parsed = true;
            }

            return icon;
        }

        protected override object Get()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            var icon = XamlTools.FromXamlString(_source);

            if (!_parsed && _foreground != null && icon is UIElement ui)
            {
                XamlTools.SetBinding(ui, _foreground.ToColor());
                _source = XamlWriter.Save(ui);
                _parsed = true;
            }

            return icon;
        }
    }
}