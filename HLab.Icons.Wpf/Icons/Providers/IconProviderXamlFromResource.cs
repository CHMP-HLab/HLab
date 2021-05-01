using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromResource : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
        private readonly Color? _foreColor;
        private bool _parsed = false;
        private string _source;
 
        public IconProviderXamlFromResource(ResourceManager resourceManager, string name, Color? foreColor)
        { 
            _resourceManager = resourceManager; 
            _name = name;
            _foreColor = foreColor;
        }
        public async Task<object> GetAsync()
        {

            if (string.IsNullOrWhiteSpace(_name)) return null;

            if (_parsed)
            {
                return await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(false);
            }

            await using var xamlStream = _resourceManager.GetStream(_name);
            if (xamlStream == null) return null;


            var icon = await XamlTools.FromXamlStreamAsync(xamlStream).ConfigureAwait(true);
            if (icon != null)
            {
                if (_foreColor != null)
                {
                    XamlTools.SetBinding(icon, _foreColor);
                    _source = XamlWriter.Save(icon);
                }
            } 
            _parsed = true;

            return icon;
        }

        public object Get()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            if (_parsed)
            {
                return XamlTools.FromXamlStringAsync(_source).ConfigureAwait(false);
            }

            using var xamlStream = _resourceManager.GetStream(_name);
            if (xamlStream == null) return null;


            var icon = XamlTools.FromXamlStream(xamlStream);
            if (icon != null)
            {
                if (_foreColor != null)
                {
                    XamlTools.SetBinding(icon, _foreColor);
                    _source = XamlWriter.Save(icon);
                }
            } 
            _parsed = true;

            return icon;
        }
    }
}