using Avalonia;
using Avalonia.Controls.Skia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using HLab.Base.Avalonia.Extensions;
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

    public object Get(IBrush? foreground)
    {
        //var svg = new global::Avalonia.Svg.Skia.Svg(_uri){Path = _uri.AbsoluteUri};


        var color = (foreground switch
        {
            ISolidColorBrush s => s.Color,
            IGradientBrush g => g.GradientStops.AverageColor(),
            //ConicGradientBrush cg => cg.GradientStops.Average(),
            _ => Colors.Red
        });

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

    public Task<object> GetAsync(IBrush? foreground)
    {
        return Task.FromResult(Get(foreground));
    }

    public Task<string> GetTemplateAsync(IBrush? foreground)
    {
        return Task.FromResult($"<Svg Path=\"{_uri.AbsoluteUri}\"/>");
    }
}