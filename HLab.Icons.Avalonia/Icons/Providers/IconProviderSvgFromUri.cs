using Avalonia;
using Avalonia.Controls.Skia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using HLab.Base.Avalonia.Extensions;
using HLab.ColorTools.Avalonia;
using HLab.Mvvm.Annotations;
using Svg.Skia;

namespace HLab.Icons.Avalonia.Icons.Providers;

public class IconProviderSvgFromUri : IIconProvider
{
    readonly Uri _uri;
    string _source;

    public IconProviderSvgFromUri(Uri uri, Color? foreColor)
    {
        _uri = uri;
    }

    public object Get(uint foregroundColor = 0)
    {
        //var svg = new global::Avalonia.Svg.Skia.Svg(_uri){Path = _uri.AbsoluteUri};

        var color = foregroundColor.ToAvaloniaColor();

        var foregroundString = $"{color.R:X2}{color.G:X2}{color.B:X2}";

        var stream = AssetLoader.Open(_uri);
        var reader = new StreamReader(stream);
        _source = reader.ReadToEnd();

        _source = _source.Replace("\"#000000\"",$"\"#{foregroundString}\"");

        _source = _source.Replace(":#000000",$":#{foregroundString}");
        //_source = _source.Replace(":#000000",":#ffffff");

        if (!_source.Contains(foregroundString))
        {

        }
 


        var svg = new SKSvg();

        var picture = new SKPictureControl(){Picture = svg.FromSvg(_source)};


        return picture;
    }

    public Task<object> GetAsync(uint foreground = 0)
    {

        return Task.FromResult(Get(foreground));
    }

    public Task<string> GetTemplateAsync(uint foreground = 0)
    {
        return Task.FromResult($"<Svg Path=\"{_uri.AbsoluteUri}\"/>");
    }
}