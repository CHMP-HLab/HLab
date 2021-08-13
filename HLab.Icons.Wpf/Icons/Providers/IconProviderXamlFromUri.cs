using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromUri : IconProvider, IIconProvider
    {
        private readonly Uri _uri;
        public IconProviderXamlFromUri(Uri uri)
        {
            _uri = uri;
        }

        
        protected override async Task<object> GetAsync()
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            var icon = Application.LoadComponent(_uri);
            if(icon is DependencyObject d)
                XamlTools.SetBinding(d,Colors.Black);

            return icon;
        }

        protected override object Get()
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            var icon = Application.LoadComponent(_uri);
            if(icon is DependencyObject d)
                XamlTools.SetBinding(d,Colors.Black);

            return icon;
        }
    }
}
