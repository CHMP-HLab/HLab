using Avalonia.Controls.Skia;
using Avalonia.Media;
using Avalonia.Platform;
using HLab.Mvvm.Annotations;
using Svg.Skia;

namespace HLab.Icons.Avalonia.Icons.Providers;

public class IconProviderSvgFromUri(Uri uri, Color? foreColor) : IIconProvider
{
    string _source ="";

    public object Get(uint foregroundColor = 0)
    {
        var color = Color.FromUInt32(foregroundColor);

        var foregroundString = $"{color.R:X2}{color.G:X2}{color.B:X2}";

        var stream = AssetLoader.Open(uri);
        var reader = new StreamReader(stream);
        _source = reader.ReadToEnd();

        _source = _source.Replace("\"#000000\"",$"\"#{foregroundString}\"");

        _source = _source.Replace(":#000000",$":#{foregroundString}");

        var svg = new SKSvg().FromSvg(_source);

        return new SKPictureControl(){Picture = svg};
    }

    public Task<object> GetAsync(uint foreground = 0)
    {

        return Task.FromResult(Get(foreground));
    }

    public Task<string> GetTemplateAsync(uint foreground = 0)
    {
        return Task.FromResult($"<Svg Path=\"{uri.AbsoluteUri}\"/>");
    }
}