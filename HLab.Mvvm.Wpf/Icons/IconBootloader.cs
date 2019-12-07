﻿using System;
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
                        var r = ((DictionaryEntry)rkey).Key.ToString().ToLower();

                        var resourcePath = r.Replace(assembly.ManifestModule.Name.Replace(".exe", "") + ".", "");
                        if (resourcePath.EndsWith(".xaml"))
                        {
                            var n = resourcePath.Remove(resourcePath.Length-5);
                            _icons.AddIconProvider(n, new IconProviderXaml(resourceManager, resourcePath, _icons));
                        }
                        else if (resourcePath.EndsWith(".svg"))
                        {
                            var n = resourcePath.Remove(resourcePath.Length-4);
                            _icons.AddIconProvider(n, new IconProviderSvg(resourceManager, resourcePath, _icons));
                        }
                        else if (resourcePath.EndsWith(".baml"))
                        {
                            var n = resourcePath.Remove(resourcePath.Length-5);
                            _icons.AddIconProvider(n, new IconProviderXaml(resourceManager, resourcePath, _icons));
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