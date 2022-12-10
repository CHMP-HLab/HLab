using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using HLab.Base;
using HLab.Core;
using HLab.Icons.Annotations.Icons;
using Mages.Core;
using HashCode = System.HashCode;
using Size = System.Drawing.Size;

namespace HLab.Icons.Wpf.Icons;

public class IconService : Service, IIconService
{
    readonly ConcurrentDictionary<string, IIconProvider> _cache = new();

    readonly AsyncDictionary<string, object> _templates = new();

    public async Task<object> GetIconTemplateAsync(string path)
    {
        return await _templates.GetOrAddAsync(
            path, 
            p => BuildAsync(path)
        );
    }

    async Task<object> BuildAsync(string path, object foreground = null)
    {
        var template =
            await await Application.Current.Dispatcher.InvokeAsync(() => BuildIconTemplateAsync(path));
        return template;
    }

    public async Task<object> BuildIconTemplateAsync(string path)
    {
        if (path == null) return null;
        var result = "";
        var paths = path.Split('|');
            
        foreach (var p in paths)
        {
            var icon = await GetSingleIconTemplateAsync(p);
            icon ??= "";

            // Remove <?xml ?> tag
            var i = icon.IndexOf("?>", StringComparison.Ordinal);
            if (i >= 0)
            {
                icon = icon[(i+2)..];
            }

            if (result == "")
            {
                result = icon; 
                continue;
            }

            result = $@"<Grid HorizontalAlignment=""Center"" VerticalAlignment=""Center"">
                    {result}
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width=""*""/>
                            <ColumnDefinition Width=""*""/>
                    </Grid.ColumnDefinitions>
                        <Grid.RowDefinitions>
                            <RowDefinition Height=""*""/>
                            <RowDefinition Height=""*""/>
                    </Grid.RowDefinitions>
                        <ContentControl Grid.Row=""1"" Grid.Column=""1"">
                            {icon}
                        </ContentControl>
                    </Grid>
                </Grid>";
        }

        result = $@"<DataTemplate 
                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                xmlns:icons=""clr-namespace:HLab.Icons.Wpf.Icons;assembly=HLab.Icons.Wpf"">
                {result}
            </DataTemplate>";

        // replace black color with binding to foreground
        // TODO : use clever algo to parse xml, cause "Black" might be used inside strings
        const string r = "\"{Binding Foreground, RelativeSource={RelativeSource AncestorType={x:Type icons:IconView}}}\"";

        result = result.Replace("\"Black\"", r);
        result = result.Replace("\"#FF000000\"", r);
        result = result.Replace("\"#000000\"", r);

        //when assembling multiple icons names can get duplicated but we don't need them
        result = Regex.Replace(result, "Name *?= *?\".*?\"", "");
        //result = Regex.Replace(result, "<", "");

        try
        {
            return XamlReader.Parse(result);
        }
        catch
        {
            var r1 = result;
        }

        return "";
    }


    object GetSingleIcon(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (_cache.TryGetValue(path.ToLower(), out var iconProvider))
        {
            var icon = iconProvider.Get();
            return icon;
        }

        if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
        {
            var icon = iconProviderDefault.Get();
            return icon;
        }

        Debug.Print("Icon not found : " + path);

        return null;
    }

    async Task<object> GetSingleIconAsync(string path, object foreground = null, Size size = default)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (_cache.TryGetValue(path.Trim().ToLower(), out var iconProvider))
        {
            var icon = await iconProvider.GetAsync().ConfigureAwait(true);
            return icon;
        }

        if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
        {
            var icon = await iconProviderDefault.GetAsync().ConfigureAwait(true);
            return icon;
        }

        Debug.Print("Icon not found : " + path);

        return null;
    }
    async Task<string> GetSingleIconTemplateAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (_cache.TryGetValue(path.Trim().ToLower(), out var iconProvider))
        {
            var icon = await iconProvider.GetTemplateAsync();

            Regex.Replace(icon, @"<\?.*?\?>" ,"");//    <?xml version="1.0" encoding="UTF-8"?>
            return icon;
        }

        if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
        {
            var icon = await iconProviderDefault.GetTemplateAsync();
            return icon;
        }

        Debug.Print("Icon not found : " + path);

        return "";
    }

    public void AddIconProvider(string name, IIconProvider provider)
    {
        _cache.AddOrUpdate(name.ToLower(), n => provider, (n, p) => provider);
    }

    public IIconProvider GetIconProvider(string name)
    {
        throw new System.NotImplementedException();
    }

}