using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HLab.Base;
using HLab.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Avalonia;

public class IconServiceDesign : IconService
{
    public IconServiceDesign()
    {
        if(!Design.IsDesignMode) throw new InvalidOperationException("Only for design mode");
    }

}


public class IconService : Service, IIconService
{
    readonly ConcurrentDictionary<string, IIconProvider> _cache = new();

    readonly AsyncDictionary<string, object> _templates = new();
    readonly AsyncDictionary<string, string> _source = new();

    public async Task<object?> GetIconTemplateAsync(string path, uint foreground = 0)
    {
        return await _templates.GetOrAddAsync(
            path,
            p => BuildTemplateAsync(path, foreground)
        );
    }

    //public Task<object> GetIconAsync(string path) => GetObjectAsync(path, "ContentControl");
    public Task<object?> GetIconAsync(string path, uint foreground = 0)
    {
        return BuildIconAsync(path, foreground);
    }

    async Task<object> BuildTemplateAsync(string path, uint foreground = 0)
    {
        var template = await GetObjectAsync(path, "Template", foreground);
        // await await Application.Current.Dispatcher.InvokeAsync(() => BuildIconTemplateAsync(path));
        return template;
    }

    async Task<object> GetObjectAsync(string path, string container, uint foreground = 0)
    {
        var result = await _source.GetOrAddAsync(
            path,
            p => BuildIconSourceAsync(path, foreground)
        );

        result = $"""
             <{container} 
                 xmlns="https://github.com/avaloniaui"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:icons="clr-namespace:HLab.Icons.Avalonia.Icons;assembly=HLab.Icons.Avalonia">
                 {result}
             </{container}>
             """;

        try
        {
            var obj = AvaloniaRuntimeXamlLoader.Parse(result);
            return obj;
        }
        catch
        {
#if DEBUG
            var r1 = result;
#endif
        }

        return "";
    }


    public async Task<object?> BuildIconAsync(string path, uint foreground = 0)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;

        var paths = path.Split('|');

        Control? icon = null;

        foreach (var p in paths.Reverse())
        {
            if (icon == null)
            {
                icon = (Control)await GetSingleIconAsync(p,foreground);
                continue;
            }

            var i = (Control)await GetSingleIconAsync(p, foreground);

            icon.SetValue(Grid.ColumnProperty, 1);
            icon.SetValue(Grid.RowProperty, 1);

            icon = new Grid
            {
                Children =
                {
                    i,
                    new Grid
                    {
                        ColumnDefinitions = [new (GridLength.Star),new (GridLength.Star)],
                        RowDefinitions = [new (GridLength.Star),new (GridLength.Star)],
                        Children = { icon }
                    }
                }
            };
        }

        return icon;
    }

    public async Task<string> BuildIconSourceAsync(string path, uint foreground = 0)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;

        var result = string.Empty;
        var paths = path.Split('|');

        foreach (var p in paths)
        {
            var icon = await GetSingleIconTemplateAsync(p, foreground);
            icon ??= "";

            // Remove <?xml ?> tag
            var i = icon.IndexOf("?>", StringComparison.Ordinal);
            if (i >= 0)
            {
                icon = icon[(i + 2)..];
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


        // replace black color with binding to foreground
        // TODO : use clever algo to parse xml, cause "Black" might be used inside strings
        const string r = "\"{Binding Foreground, RelativeSource={RelativeSource AncestorType={x:Type icons:IconView}}}\"";

        result = result.Replace("\"Black\"", r);
        result = result.Replace("\"#FF000000\"", r);
        result = result.Replace("\"#000000\"", r);

        //when assembling multiple icons names can get duplicated but we don't need them
        result = Regex.Replace(result, "Name *?= *?\".*?\"", "");
        //result = Regex.Replace(result, "<", "");

        return result;
    }


    object GetSingleIcon(string path, uint foreground = 0)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (_cache.TryGetValue(path.ToLower(), out var iconProvider))
        {
            var icon = iconProvider.Get(foreground);
            return icon;
        }

        if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
        {
            var icon = iconProviderDefault.Get(foreground);
            return icon;
        }

        Debug.Print("Icon not found : " + path);

        return null;
    }

    async Task<object> GetSingleIconAsync(string path, uint foreground = 0)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (_cache.TryGetValue(path.Trim().ToLower(), out var iconProvider))
        {
            var icon = await iconProvider.GetAsync(foreground).ConfigureAwait(true);
            return icon;
        }

        if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
        {
            var icon = await iconProviderDefault.GetAsync(foreground).ConfigureAwait(true);
            return icon;
        }

        Debug.Print("Icon not found : " + path);

        return null;
    }

    async Task<string> GetSingleIconTemplateAsync(string path, uint foreground = 0)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;

        if (_cache.TryGetValue(path.Trim().ToLower(), out var iconProvider))
        {
            var icon = await iconProvider.GetTemplateAsync(foreground);

            Regex.Replace(icon, @"<\?.*?\?>", "");//    <?xml version="1.0" encoding="UTF-8"?>
            return icon;
        }

        if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
        {
            var icon = await iconProviderDefault.GetTemplateAsync(foreground);
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