using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Xsl;

using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Icons
{
    public class IconBootloader : IBootloader
    {

        private readonly IIconService _icons;
        [Import]
        public IconBootloader(IIconService icons)
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
                        var r = ((DictionaryEntry)rkey).Key.ToString();

                        var s = r.Replace(assembly.ManifestModule.Name.Replace(".exe", "") + ".", "");
                        if (r.EndsWith(".xaml"))
                        {
                            var n = s.Replace(".xaml", "").ToLower();
                            _icons.AddIconProvider(n, new IconProviderXaml(assembly, n, _icons));
                        }
                        else if (r.EndsWith(".svg"))
                        {
                            var n = s.Replace(".svg", "").ToLower();
                            _icons.AddIconProvider(n, new IconProviderSvg(assembly, n, _icons));
                        }
                    }
                }
                catch (System.Resources.MissingManifestResourceException ex)
                {
                }
                //var ressources = assembly.GetManifestResourceNames();
            }
        }
    }

    [Export(typeof(IIconService)), Singleton]
    public class IconService : IIconService
    {
        private readonly ConcurrentDictionary<string, IIconProvider> _cache = new ConcurrentDictionary<string, IIconProvider>();

        public dynamic Icon { get; }

        public IconService()
        {
            Icon = new IconHelper(this);
        }

        public object GetIcon(string name, string backMatch, string foreMatch)
        {

            if (string.IsNullOrWhiteSpace(name)) return null;

            if (_cache.TryGetValue(name.ToLower(), out var iconProvider))
            {
                return iconProvider?.Get(backMatch, foreMatch);
            }

            return null;
        }

        public void AddIconProvider(string name, IIconProvider provider)
        {
            _cache.TryAdd(name, provider);
        }

        private XslCompiledTransform _transformSvg;
        public XslCompiledTransform TransformSvg
        {
            get
            {
                if (_transformSvg == null)
                {
                    using (var xslStream = Assembly.GetAssembly(typeof(IconProviderSvg))
                        .GetManifestResourceStream("HLab.Mvvm.Wpf.Icons.svg2xaml.xsl"))
                    {
                        if (xslStream == null) throw new IOException("xsl file not found");
                        using (var stylesheet = XmlReader.Create(xslStream))
                        {
                            var settings = new XsltSettings { EnableDocumentFunction = true };
                            _transformSvg = new XslCompiledTransform();
                            _transformSvg.Load(stylesheet, settings, new XmlUrlResolver());
                        }
                    }
                }
                return _transformSvg;
            }
        }
        private XslCompiledTransform _transformHtml;

        private XslCompiledTransform TransformHtml
        {
            get
            {
                if (_transformHtml == null)
                {
                    using (var xslStream = Assembly.GetAssembly(GetType())
                        .GetManifestResourceStream("HLab.Mvvm.Wpf.Icons.html2xaml.xslt"))
                    {
                        if (xslStream == null) throw new IOException("xsl file not found");
                        using (var stylesheet = XmlReader.Create(xslStream))
                        {
                            var settings = new XsltSettings { EnableDocumentFunction = true };
                            _transformHtml = new XslCompiledTransform();
                            _transformHtml.Load(stylesheet, settings, new XmlUrlResolver());
                        }
                    }
                }
                return _transformHtml;
            }
        }

        public object GetFromHtml(string html)
        {
            TextBlock textBlock = null;
            //Application.Current.Dispatcher.Invoke(
            //() =>
            {
                using (var s = new MemoryStream())
                {
                    using (var stringReader = new StringReader
                      (html))
                    {
                        using (var htmlReader = XmlReader.Create(stringReader))
                        {
                            try
                            {
                                using (var w = XmlWriter.Create(s))
                                {
                                    TransformHtml.Transform(htmlReader, w);
                                }
                            }
                            catch (XmlException)
                            {
                                using (var sw = new StreamWriter(s))
                                {
                                    sw.Write(Regex.Replace(html, "<.*?>", string.Empty));
                                }
                            }
                            catch (ObjectDisposedException)
                            {
                                using (var sw = new StreamWriter(s))
                                {
                                    sw.Write(Regex.Replace(html, "<.*?>", string.Empty));
                                }
                            }

                            try
                            {
                                s.Seek(0, SeekOrigin.Begin);

                                var sz = Encoding.UTF8.GetString(s.ToArray());
                                using (var reader = XmlReader.Create(s))
                                {
                                    textBlock = (TextBlock)System.Windows.Markup.XamlReader.Load(reader);
                                    // Code to run on the GUI thread.
                                }

                            }
                            catch (IOException)
                            {
                            }
                            catch (ObjectDisposedException)
                            {

                            }
                        }
                    }
                }
            }/*);*/
            return textBlock;
        }



        public BitmapSource GetIconBitmap(string name, Size size)
        {
            var visual = (UIElement)GetIcon(name, "", "");

            var grid = new Grid { Width = size.Width, Height = size.Height };
            var viewbox = new Viewbox
            {
                Width = size.Width,
                Height = size.Height,
                Child = visual
            };


            grid.Children.Add(viewbox);

            grid.Measure(size);
            grid.Arrange(new Rect(size));
            //grid.UpdateLayout();

            var renderBitmap =
                new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96,
                96,
                PixelFormats.Pbgra32);
            renderBitmap.Render(grid);
            return BitmapFrame.Create(renderBitmap);
        }

        public object FromSvgString(string svg, string foreMatch, string backMatch)
        {
            if (svg == null) return null;
            byte[] byteArray = Encoding.ASCII.GetBytes(svg);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                return FromSvgStream(stream, foreMatch, backMatch);
            }
        }

        public UIElement FromXamlStream(Stream xamlStream, string foreMatch, string backMatch)
        {
            var foreMatch2 = foreMatch?.Replace("#FF", "#");
            var backMatch2 = backMatch?.Replace("#FF", "#");

            using (StreamReader reader = new StreamReader(xamlStream))
            {
                var xaml = reader.ReadToEnd();

                if (!string.IsNullOrEmpty(foreMatch))
                    xaml = xaml
                         .Replace(foreMatch,
                        "{Binding Path=Foreground, RelativeSource={RelativeSource AncestorType={x:Type Control}},FallbackValue=black}")
                       .Replace(foreMatch2,
                        "{Binding Path=Foreground, RelativeSource={RelativeSource AncestorType={x:Type Control}},FallbackValue=black}")
                        ;

                if (!string.IsNullOrEmpty(backMatch))
                    xaml = xaml
                         .Replace(backMatch,
                        "{Binding Path=Background, RelativeSource={RelativeSource AncestorType={x:Type Control}},FallbackValue=white}")
                       .Replace(backMatch2,
                        "{Binding Path=Background, RelativeSource={RelativeSource AncestorType={x:Type Control}},FallbackValue=white}")
                        ;


                try
                {
                    var icon = (UIElement)XamlReader.Parse(xaml);
                    return icon;
                }
                catch (XamlParseException e)
                {
                    return new Viewbox { ToolTip = "Error" };
                }
                catch (IOException)
                {
                    return null;
                }
            }
        }

        public UIElement FromSvgStream(Stream svg, string foreMatch, string backMatch)
        {
            if (svg == null) return null;
            using (var s = new MemoryStream())
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 1024
                };

                using (var svgReader = XmlReader.Create(svg, settings))
                {
                    try
                    {
                        using (var w = XmlWriter.Create(s))
                        {
                            TransformSvg.Transform(svgReader, w);
                        }

                        s.Seek(0, SeekOrigin.Begin);
                        //var sz = Encoding.UTF8.GetString(s.ToArray());

                        return FromXamlStream(s, foreMatch, backMatch);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                }
            }
        }
    }
}
