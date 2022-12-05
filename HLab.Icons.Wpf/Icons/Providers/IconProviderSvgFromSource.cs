using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using HLab.ColorTools.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderSvgFromSource : IconProvider, IIconProvider
    {
        readonly string _name;
        string _source;
        bool _parsed = false;

        public IconProviderSvgFromSource(string source, string name, int? foreColor)
        {
            _source = source; 
            _name = name;
        }

        public override object Get()
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

            return icon;
        }
        
        protected override async Task<object> GetActualAsync()
        {
            if (string.IsNullOrWhiteSpace(_source)) return null;

            object icon;
            if (_parsed)
            {
                icon = await XamlTools.FromXamlStringAsync(_source);
            }
            else
            {
                icon = await XamlTools.FromSvgStringAsync(_source);
                await Application.Current.Dispatcher.InvokeAsync(
                    ()=>_source = XamlWriter.Save(icon),XamlTools.Priority2
                    );
                _parsed = true;
            }
            return icon;
        }

        public override async Task<string> GetTemplateAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return "";
            while (!_parsed)
            {
                var icon = await GetAsync();
            }
            return _source;
        }
    }
}