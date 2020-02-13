using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderSvgFromSource : IIconProvider
    {
        private readonly string _name;
        private readonly string _source;
 
        public IconProviderSvgFromSource(string source, string name)
        { _source = source; _name = name;}
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            return await XamlTools.FromSvgStringAsync(_source).ConfigureAwait(false);
        }
    }
}