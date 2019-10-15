using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Icons
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
        public object Get(string foreMatch, string backMatch)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            //using (var s = new MemoryStream())
            {
                var resourceManager = new ResourceManager(_assembly.GetName().Name + ".g", _assembly);
                using (
                    var xamlStream = resourceManager.GetStream(_name + ".xaml"))
                {
                    if (xamlStream == null) return null;
                    return (_icons as IconService)?.FromXamlStream(xamlStream, foreMatch, backMatch);
                }
            }
        }
    }
}
