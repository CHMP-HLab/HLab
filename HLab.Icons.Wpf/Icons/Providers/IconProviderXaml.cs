using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HLab.Icons.Wpf.Icons.Providers;

public abstract class IconProviderXaml : IconProvider
{
    string _sourceXaml;

    protected IconProviderXaml(string sourceXaml)
    {
        _sourceXaml = sourceXaml;
    }

    protected override object? GetIcon() => XamlTools.FromXamlString(_sourceXaml);

    protected override async Task<object?> GetIconAsync() => await XamlTools.FromXamlStringAsync(_sourceXaml).ConfigureAwait(true);

    public override async Task<string> GetTemplateAsync() => _sourceXaml;

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
        
    protected override object? GetIcon()
    {
        if (_parsed) return base.GetIcon();

        if (ParseIcon() is not { } icon) return null;

        SetSource(XamlWriter.Save(icon));
        _parsed = true;

        return icon;
    }

    protected override async Task<object?> GetIconAsync()
    {
        if (_parsed) return await base.GetIconAsync();

        if (await ParseIconAsync() is not { } icon) return null;

        await Application.Current.Dispatcher.InvokeAsync(
            ()=>SetSource(XamlWriter.Save(icon)),XamlTools.Priority2);
        _parsed = true;

        return icon;
    }

    public override async Task<string> GetTemplateAsync()
    {
        while (!_parsed)
        {
            await GetIconAsync();
        }
        return await base.GetTemplateAsync();
    }

}