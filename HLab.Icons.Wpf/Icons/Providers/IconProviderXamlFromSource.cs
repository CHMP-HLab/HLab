using System.Threading.Tasks;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromSource : IIconProvider
    {
        private readonly string _name;
        private readonly string _source;
        private readonly int? _foreground;
 
        public IconProviderXamlFromSource(string source, string name, int? foreground)
        { 
            _source = source; 
            _name = name;
            _foreground = foreground;
        }

        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            return await XamlTools.FromXamlStringAsync(_source,_foreground).ConfigureAwait(false);
        }
    }
}