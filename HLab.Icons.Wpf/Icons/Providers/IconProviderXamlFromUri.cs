using System;
using System.Threading.Tasks;
using System.Windows;
using HLab.Icons.Annotations;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;

namespace HLab.Icons.Wpf.Providers
{
    public class IconProviderXamlFromUri : IIconProvider
    {
        private readonly Uri _uri;
        public IconProviderXamlFromUri(Uri uri)
        {
            _uri = uri;
        }

        
        public async Task<object> GetAsync()
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            var icon = System.Windows.Application.LoadComponent(_uri);
            if(icon is DependencyObject d)
                XamlTools.SetBinding(d);

            return icon;
        }
    }
}
