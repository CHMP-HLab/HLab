using System.Reflection;
using System.Resources;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconProviderSvg : IIconProvider
    {
        private readonly Assembly _assembly;
        private readonly string _name;
        private readonly IIconService _icons;
        public IconProviderSvg(Assembly assembly, string name, IIconService icons)
        { _assembly = assembly; _name = name; _icons = icons; }



        public object Get(string foreMatch, string backMatch)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

                var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);

                using (
                        //var svg =
                        //    _assembly.GetManifestResourceStream(_assembly.GetName().Name + /*".Icons." +*/
                        //                                        _name.Replace("/", ".") + ".svg"))
                        var svg = resourceManager.GetStream(_name + ".svg") )
                {
                    return (_icons as IconService)?.FromSvgStream(svg, foreMatch, backMatch);
                }
            
        }




    }
}
