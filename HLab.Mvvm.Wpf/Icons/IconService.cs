using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Xsl;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
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
                        .GetManifestResourceStream("HLab.Mvvm.Icons.svg2xaml.xsl"))
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
                        .GetManifestResourceStream("HLab.Mvvm.Icons.html2xaml.xslt"))
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
                                    textBlock = (TextBlock)XamlReader.Load(reader);
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
