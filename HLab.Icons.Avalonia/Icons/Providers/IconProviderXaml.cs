﻿using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;

namespace HLab.Icons.Avalonia.Icons.Providers;

public abstract class IconProviderXaml : IconProvider
{
    string _sourceXaml;

    protected IconProviderXaml(string sourceXaml)
    {
        _sourceXaml = sourceXaml;
    }

    protected override object? GetIcon(IBrush? foreground) => XamlTools.FromXamlString(_sourceXaml);

    protected override async Task<object?> GetIconAsync(IBrush? foreground) => await XamlTools.FromXamlStringAsync(_sourceXaml).ConfigureAwait(true);

    public override async Task<string> GetTemplateAsync(IBrush? foreground) => _sourceXaml;

    protected void SetSource(string  source) => _sourceXaml = source;
}

public abstract class IconProviderXamlParser : IconProviderXaml
{
    bool _parsed = false;
    protected IconProviderXamlParser() : base("")
    {
    }

    protected abstract object? ParseIcon();
    protected abstract Task<object?> ParseIconAsync();
        
    protected override object? GetIcon(IBrush? foreground)
    {
        if (_parsed) return base.GetIcon(foreground);

        if (ParseIcon() is not { } icon) return null;

        // TODO : avalonia
        //SetSource(XamlWriter.Save(icon));
        //_parsed = true;

        return icon;
    }

    protected override async Task<object?> GetIconAsync(IBrush? foreground)
    {
        if (_parsed) return await base.GetIconAsync(foreground);

        if (await ParseIconAsync() is not { } icon) return null;
        
        // TODO : avalonia
        //await Dispatcher.UIThread.InvokeAsync(
        //    ()=>SetSource(XamlWriter.Save(icon)),XamlTools.Priority2);
        //_parsed = true;

        return icon;
    }

    public override async Task<string> GetTemplateAsync(IBrush? foreground)
    {
        while (!_parsed)
        {
            await GetIconAsync(foreground);
        }
        return await base.GetTemplateAsync(foreground);
    }

}