using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromUri : IconProvider, IIconProvider
    {
        readonly Uri _uri;
        bool _parsed = false;
        string _source;

        public IconProviderXamlFromUri(Uri uri)
        {
            _uri = uri;
        }
        public override object Get()
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            return Application.LoadComponent(_uri);;
        }
        
        protected override async Task<object> GetActualAsync()
        {
            object icon = null;
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            await Application.Current.Dispatcher.InvokeAsync(
                () => icon = Application.LoadComponent(_uri)
                ,XamlTools.Priority
                );

            return icon;
        }

        public override async Task<string> GetTemplateAsync()
        {
            while (!_parsed)
            {
                var icon = await GetActualAsync();
                if (icon == null) return "";

                await Application.Current.Dispatcher.InvokeAsync(
                    ()=> _source= XamlWriter.Save(icon),XamlTools.Priority2);
                _parsed = true;
            }
            return _source;
        }

    }
}
