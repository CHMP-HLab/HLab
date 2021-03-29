using System.Resources;
using System.Threading.Tasks;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderXamlFromResource : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
        private readonly int? _foreColor;
 
        public IconProviderXamlFromResource(ResourceManager resourceManager, string name, int? foreColor)
        { 
            _resourceManager = resourceManager; 
            _name = name;
            _foreColor = foreColor;
        }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            //var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);
            await using var xamlStream = _resourceManager.GetStream(_name);
            if (xamlStream == null) return null;

            return await XamlTools.FromXamlStreamAsync(xamlStream,_foreColor).ConfigureAwait(false);
        }
    }
}