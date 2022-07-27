using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromUri : IconProvider, IIconProvider
    {
        readonly Uri _uri;
        readonly Color _foreColor = Colors.Black;

        public IconProviderXamlFromUri(Uri uri)
        {
            _uri = uri;
        }
        
        protected override async Task<object> GetAsync(object foreground = null)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            var icon = Application.LoadComponent(_uri);

            if(icon is DependencyObject o && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor, brush);

            return icon;
        }

        protected override object Get(object foreground = null)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            var icon = Application.LoadComponent(_uri);

            if(icon is DependencyObject o && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor, brush);

            return icon;
        }
    }
}
