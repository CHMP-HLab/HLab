using Avalonia.Controls.Skia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using HLab.Mvvm.Annotations;
using Svg.Skia;

namespace HLab.Icons.Avalonia.Icons.Providers;

public class IconProviderSvgFromUri(Uri uri, Color? foreColor) : IIconProvider
{
    string _source ="";

    public object Get(uint foregroundColor = 0)
    {
        return GetAsync(foregroundColor).GetAwaiter().GetResult();
    }

    public async Task<object> GetAsync(uint foregroundColor = 0)
    {
        var color = Color.FromUInt32(foregroundColor);

        var foregroundString = $"{color.R:X2}{color.G:X2}{color.B:X2}";

        var stream = AssetLoader.Open(uri);
        var reader = new StreamReader(stream);

        _source = await reader.ReadToEndAsync();

        _source = _source.Replace("\"#000000\"",$"\"#{foregroundString}\"");

        _source = _source.Replace(":#000000",$":#{foregroundString}");

        var svg = new SKSvg().FromSvg(_source);

        if(Dispatcher.UIThread.CheckAccess()) 
            return new SKPictureControl(){Picture = svg};

        return await Dispatcher.UIThread.InvokeAsync(() => new SKPictureControl(){Picture = svg});
    }

    public Task<string> GetTemplateAsync(uint foreground = 0)
    {
        return Task.FromResult($"<Svg Path=\"{uri.AbsoluteUri}\"/>");
    }
}