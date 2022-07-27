using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HLab.Icons.Annotations.Icons;
using Color = System.Windows.Media.Color;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromResource : IconProvider, IIconProvider
    {
        readonly ResourceManager _resourceManager;
        readonly string _name;
        readonly Color? _foreColor;
        bool _parsed = false;
        string _source;
 
        public IconProviderXamlFromResource(ResourceManager resourceManager, string name, Color? foreColor)
        { 
            _resourceManager = resourceManager; 
            _name = name;
            _foreColor = foreColor;
        }
        protected override async Task<object> GetAsync(object foreground = null)
        {

            if (string.IsNullOrWhiteSpace(_name)) return null;

            object icon;
            if (_parsed)
            {
                icon = await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(true);
            }
            else
            {
                await using var xamlStream = _resourceManager.GetStream(_name);
                if (xamlStream == null) return null;
                icon = await XamlTools.FromXamlStreamAsync(xamlStream).ConfigureAwait(true);
                _source = XamlWriter.Save(icon);
                _parsed = true;
            }

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.Value, brush);

            return icon;
        }

        protected override object Get(object foreground = null)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            object icon;
            if (_parsed)
            {
                icon = XamlTools.FromXamlString(_source);
            }
            else
            {
                using var xamlStream = _resourceManager.GetStream(_name);
                if (xamlStream == null) return null;
                
                icon = XamlTools.FromXamlStream(xamlStream);
                _source = XamlWriter.Save(icon);
                _parsed = true;
            }

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.Value, brush);

            return icon;
        }
    }
}