using System.Resources;
using System.Threading.Tasks;
using HLab.Icons.Annotations;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;

namespace HLab.Icons.Wpf.Providers
{
    public class IconProviderXamlFromResource : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
 
        public IconProviderXamlFromResource(ResourceManager resourceManager, string name)
        { _resourceManager = resourceManager; _name = name; }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            //var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);
            await using var xamlStream = _resourceManager.GetStream(_name);
            if (xamlStream == null) return null;

            return await XamlTools.FromXamlStreamAsync(xamlStream).ConfigureAwait(false);
        }
    }
}