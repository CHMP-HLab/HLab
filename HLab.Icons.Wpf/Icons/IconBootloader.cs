using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using HLab.Core.Annotations;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons.Providers;

namespace HLab.Icons.Wpf.Icons
{
    public class IconBootloader : IBootloader
    {
        private readonly IIconService _icons;
        public IconBootloader(IIconService icons)
        {
            _icons = icons;
        }

        public void Load(IBootContext b)
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
                        var r = ((DictionaryEntry)rkey).Key.ToString().ToLower();

                        var resourcePath = r.Replace(assembly.ManifestModule.Name.Replace(".exe", "") + ".", "");

                        resourcePath = Uri.UnescapeDataString(resourcePath);


                        if (resourcePath.EndsWith(".xaml"))
                        {
                            var resource = resourcePath.Split('/');
                            var path = string.Join('/',resource.SkipLast(1));

                            foreach(var n in resource.Last().Split('.').SkipLast(1))
                               _icons.AddIconProvider(string.Join('/',path,n), new IconProviderXamlFromResource(resourceManager, resourcePath, Colors.Black));
                        }
                        else if (resourcePath.EndsWith(".svg"))
                        {
                            var resource = resourcePath.Split('/');
                            var path = string.Join('/',resource.SkipLast(1));

                            foreach(var n in resource.Last().Split('.').SkipLast(1))
                                _icons.AddIconProvider(string.Join('/',path,n), new IconProviderSvg(resourceManager, resourcePath, Colors.Black));
                        }
                        else if (resourcePath.EndsWith(".baml"))
                        {
                            var resource = resourcePath.Split('/');
                            var path = string.Join('/',resource.SkipLast(1));

                            foreach(var n in resource.Last().Split('.').SkipLast(1))
                                _icons.AddIconProvider(
                                    string.Join('/',path,n), 
                                    new IconProviderXamlFromUri(new Uri( $"/{assembly.FullName};component/{string.Join('.',resourcePath.Split('.').SkipLast(1))}.xaml",UriKind.Relative))
                                    );
                        }
                    }
                }
                catch (MissingManifestResourceException)
                {
                }


            }
        }
    }
}