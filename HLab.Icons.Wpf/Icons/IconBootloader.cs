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

namespace HLab.Icons.Wpf.Icons;

public class IconBootloader : IBootloader
{
    readonly IIconService _icons;
    public IconBootloader(IIconService icons)
    {
        _icons = icons;
    }

    public void Load(IBootContext b)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic))
        {
            if(assembly.FullName.Contains("Erp.Base.Wpf"))
            {}

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

                    bool HasSuffix(string path, string suffix, out string result)
                    {
                        if (!path.EndsWith(suffix))
                        {
                            result = null;
                            return false;
                        }

                        result = path[..^suffix.Length];
                        return true;
                    }
                    if (HasSuffix(resourcePath,".xaml",out var xamlPath))
                    {
                        _icons.AddIconProvider(xamlPath, new IconProviderXamlFromResource(resourceManager, resourcePath, Colors.Black));
                    }
                    else if (HasSuffix(resourcePath,".svg",out var svgPath))
                    {
                        _icons.AddIconProvider(svgPath, new IconProviderSvg(resourceManager, resourcePath, Colors.Black));
                    }
                    else if (HasSuffix(resourcePath,".baml",out var bamlPath))
                    {
                        _icons.AddIconProvider(
                            bamlPath, 
                            new IconProviderXamlFromUri(new Uri( $"/{assembly.FullName};component/{bamlPath}.xaml",UriKind.Relative))
                        );
                    }
                }
            }
            catch (MissingManifestResourceException ex)
            {
            }


        }
    }
}