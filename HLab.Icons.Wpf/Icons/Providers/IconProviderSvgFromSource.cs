using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;
using Svg;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderSvgFromSource : IIconProvider
    {
        private readonly string _name;
        private string _source;
        private readonly int? _foreColor;
        private bool _parsed = false;

        public IconProviderSvgFromSource(string source, string name, int? foreColor)
        {
            _foreColor = foreColor;
            _source = source; 
            _name = name;
        }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_source)) return null;

            if (_parsed)
                return await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(false);

            var icon = await XamlTools.FromSvgStringAsync(_source).ConfigureAwait(false);
            if (_foreColor != null && icon is UIElement ui)
            {
                XamlTools.SetBinding(ui, _foreColor.ToColor());
                _source = XamlWriter.Save(ui);
            }

            _parsed = true;
            return icon;
        }

        public object Get()
        {
            if (string.IsNullOrWhiteSpace(_source)) return null;

            if (_parsed)
                return XamlTools.FromXamlString(_source);

            var icon = XamlTools.FromSvgString(_source);
            if (_foreColor != null && icon is UIElement ui)
            {
                XamlTools.SetBinding(ui, _foreColor.ToColor());
                _source = XamlWriter.Save(ui);
            }

            _parsed = true;
            return icon;
        }
    }
}