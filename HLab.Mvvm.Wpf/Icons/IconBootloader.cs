using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    public class IconBootloader : IBootloader
    {
        private readonly IIconService _icons;
        [Import] public IconBootloader(IIconService icons)
        {
            _icons = icons;
        }

        public void Load()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic))
            {
                try
                {
                    var v = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
                    if (v?.Company == "Microsoft Corporation") continue;

                    var resourceManager = new ResourceManager(assembly.GetName().Name + ".g", assembly);
                    var resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                    foreach (var rkey in resources)
                    {
                        var r = ((DictionaryEntry)rkey).Key.ToString();

                        var s = r.Replace(assembly.ManifestModule.Name.Replace(".exe", "") + ".", "");
                        if (r.EndsWith(".xaml"))
                        {
                            var n = s.Replace(".xaml", "").ToLower();
                            _icons.AddIconProvider(n, new IconProviderXaml(assembly, n, _icons));
                        }
                        else if (r.EndsWith(".svg"))
                        {
                            var n = s.Replace(".svg", "").ToLower();
                            _icons.AddIconProvider(n, new IconProviderSvg(assembly, n, _icons));
                        }
                    }
                }
                catch (System.Resources.MissingManifestResourceException ex)
                {
                }
            }
        }
    }
}