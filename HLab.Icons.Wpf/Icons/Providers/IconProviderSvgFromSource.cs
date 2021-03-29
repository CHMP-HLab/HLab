using System.Threading.Tasks;
using HLab.Icons.Annotations;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;

namespace HLab.Icons.Wpf.Providers
{
    public class IconProviderSvgFromSource : IIconProvider
    {
        private readonly string _name;
        private readonly string _source;
        private readonly int? _foreColor;

        public IconProviderSvgFromSource(string source, string name, int? foreColor)
        {
            _source = source; 
            _name = name;
        }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            return await XamlTools.FromSvgStringAsync(_source,_foreColor).ConfigureAwait(false);
        }
    }
}