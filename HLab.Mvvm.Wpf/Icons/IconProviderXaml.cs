using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderXaml : IIconProvider
    {
        private readonly Assembly _assembly;
        private readonly string _name;
        private readonly IIconService _icons;
        //public IconProviderXaml(Assembly assembly, string name)
        //{
        //    _assembly = assembly;
        //    _uri = new Uri("/" + assembly.GetName().Name + ";component/" + name.Replace(".","/") + ".xaml", UriKind.RelativeOrAbsolute);
        //}
        public IconProviderXaml(Assembly assembly, string name, IIconService icons)
        { _assembly = assembly; _name = name; _icons = icons; }
        public async Task<object> Get(string foreMatch, string backMatch)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);
            await using (
                var xamlStream = resourceManager.GetStream(_name + ".xaml"))
            {
                if (xamlStream == null) return null;

                if(_icons is IconService icons)
                    return await icons.FromXamlStream(xamlStream, foreMatch, backMatch);

                return null;
            }
        }
    }
}
