using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderSvg : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
        private readonly IIconService _icons;
        public IconProviderSvg(ResourceManager resourceManager, string name, IIconService icons)
        { _resourceManager = resourceManager; _name = name; _icons = icons; }



        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

                //var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);

                await using var svg = _resourceManager.GetStream(_name);
                if(_icons is IconService icon)
                    return await icon.FromSvgStreamAsync(svg).ConfigureAwait(false);

                return null;
        }




    }
}
