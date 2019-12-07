using System.Resources;
using System.Threading.Tasks;
using System.Windows.Media;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderXaml : IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
        private readonly IIconService _icons;
        //public IconProviderXaml(Assembly assembly, string name)
        //{
        //    _assembly = assembly;
        //    _uri = new Uri("/" + assembly.GetName().Name + ";component/" + name.Replace(".","/") + ".xaml", UriKind.RelativeOrAbsolute);
        //}
        public IconProviderXaml(ResourceManager resourceManager, string name, IIconService icons)
        { _resourceManager = resourceManager; _name = name; _icons = icons; }
        public async Task<object> GetAsync()
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            //var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);
            await using var xamlStream = _resourceManager.GetStream(_name);
            if (xamlStream == null) return null;

            if(_icons is IconService icons)
                return await icons.FromXamlStreamAsync(xamlStream).ConfigureAwait(false);

            return null;
        }
    }
}
