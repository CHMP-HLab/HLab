using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Avalonia.Media;
using Avalonia.Platform;
using HLab.Base.Extensions;
using HLab.Core.Annotations;
using HLab.Icons.Avalonia.Icons.Providers;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Avalonia.Icons;

public class IconBootloader(IIconService icons) : IBootloader
{
    public async Task LoadAsync(IBootContext b)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic))
        {
            var v = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            switch (v?.Company)
            {
                case "Microsoft Corporation":
                case ".NET Foundation and Contributors":
                case "Avalonia Team":
                    continue;
            }

            //                var t = assembly.ExportedTypes.FirstOrDefault();
            //                var resourceManager = new ResourceManager(t);
            //                var resourceManager = new ResourceManager(assembly.GetName().Name + ".g", assembly);
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                Debug.WriteLine(resourceName);

                if (resourceName.HasSuffix(".resources", out var resource))
                    ParseResourceSet(assembly, resource);
                else
                {
                    ParseResource(assembly, resourceName, new ResourceManager(resourceName, assembly));
                }

            }

            var name = assembly.GetName().Name;
            var uris = AssetLoader.GetAssets(new Uri($"avares://{name}/"),null);

            if (uris == null) continue;

            foreach (var uri in uris)
            {
                if (!uri.AbsolutePath.HasPrefix("/Assets/", out var path)) continue;

                if (!path.HasSuffix(".svg", out var key)) continue;

                var paths = key.Split('/');
                var last = paths.Last();
                var prefix = string.Join('/', paths.SkipLast(1));

                foreach (var k in last.Split('.'))
                {
                    icons.AddIconProvider($"{prefix}/{k}", new IconProviderSvgFromUri(uri, Colors.Black));
                }
            }
        }
    }

    void ParseResourceSet(Assembly assembly, string resourceName)
    {
        try
        {
            var resourceManager = new ResourceManager(resourceName, assembly);
            var resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            if (resources == null) return;

            foreach (var rkey in resources.OfType<DictionaryEntry>())
            {
                var r = rkey.Key.ToString()?.ToLower();
                Debug.WriteLine(r);

                ParseResource(assembly, r, resourceManager);

            }
        }
        catch (MissingManifestResourceException ex)
        {
        }
    }


    void ParseResource(Assembly assembly, string resource, ResourceManager resourceManager)
    {

        if(assembly.ManifestModule.Name.HasSuffix(".exe", out var exe))
            resource = resource.Replace($"{exe}.","");

        if(assembly.ManifestModule.Name.HasSuffix(".dll", out var dll))
            resource = resource.Replace($"{dll}.","");


        resource = Uri.UnescapeDataString(resource);


        if (resource.HasSuffix(".xaml", out var xamlPath))
        {
            icons.AddIconProvider(xamlPath,
                new IconProviderXamlFromResource(resourceManager, resource, Colors.Black));
        }

        else if (resource.HasSuffix(".svg", out var svgPath))
        {
            icons.AddIconProvider(svgPath, new IconProviderSvg(resourceManager, resource, Colors.Black));
        }

        else if (resource.HasSuffix(".baml", out var bamlPath))
        {
            icons.AddIconProvider(
                bamlPath,
                new IconProviderXamlFromUri(new Uri($"/{assembly.FullName};component/{bamlPath}.xaml",
                    UriKind.Relative))
            );
        }
    }
}