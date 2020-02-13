using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderXamlFromSource : IIconProvider
    {
        private readonly string _name;
        private readonly string _source;
 
        public IconProviderXamlFromSource(string source, string name)
        { _source = source; _name = name; }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            return await XamlTools.FromXamlStringAsync(_source).ConfigureAwait(false);
        }
    }
}