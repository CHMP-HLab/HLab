using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public async Task<object> GetIconAsync(string path)
        {

            if (string.IsNullOrWhiteSpace(path)) return null;

            if (_cache.TryGetValue(path.ToLower(), out var iconProvider))
            {
                return await iconProvider.GetAsync().ConfigureAwait(false);
            }

            Debug.Print("Icon not found : " + path);

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



        public async Task<BitmapSource> GetIconBitmapAsync(string name, Size size)
        {
            var visual =(UIElement) await GetIconAsync(name).ConfigureAwait(true);

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

        public async Task<object> FromSvgStringAsync(string svg)
        {
            if (svg == null) return null;
            var byteArray = Encoding.ASCII.GetBytes(svg);
            await using var stream = new MemoryStream(byteArray);
            return await FromSvgStreamAsync(stream).ConfigureAwait(false);
        }

        public async Task<UIElement> FromXamlStreamAsync(Stream xamlStream)
        {
            var foreColor = Colors.Black;
            var backColor = Colors.White;

            try
            {
                var tcs=new TaskCompletionSource<UIElement>();
                var xr = new XamlReader();

                object obj = null;
                
                xr.LoadCompleted += (o, e) =>
                {
                    if (!(obj is UIElement icon)) return;
                    tcs.SetResult(icon);
                    SetBinding(icon,foreColor,backColor);
                };

                obj = xr.LoadAsync(xamlStream);

                return await tcs.Task.ConfigureAwait(false);
            }
            catch (XamlParseException e)
            {
                return new Viewbox { ToolTip = e.Message };
            }
            catch (IOException e)
            {
                return new Viewbox { ToolTip = e.Message };
            }
        }

        private static void SetBinding(DependencyObject ui, Color foreColor, Color backColor)
        {
            var foreBinding = new Binding("Foreground")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                    typeof(Control), 1)
            };

            var backBinding = new Binding("Background")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                    typeof(Control), 1)
            };

            SetBinding(ui,foreColor,backColor,foreBinding,backBinding);
        }

        private static void SetBinding(DependencyObject ui, Color foreColor, Color backColor, BindingBase foreBinding, BindingBase backBinding)
        {
            if (ui is System.Windows.Shapes.Shape shape)
            {
                if (shape.Fill is SolidColorBrush fillBrush)
                {
                    if (fillBrush.Color == foreColor)
                    {
                        shape.SetBinding(System.Windows.Shapes.Shape.FillProperty, foreBinding);
                    }
                    else if (fillBrush.Color == backColor)
                    {
                        shape.SetBinding(System.Windows.Shapes.Shape.FillProperty, backBinding);
                    }
                }
                if (shape.Stroke is SolidColorBrush strokeBrush)
                {
                    if (strokeBrush.Color == foreColor)
                    {
                        shape.SetBinding(System.Windows.Shapes.Shape.StrokeProperty, foreBinding);
                    }
                    else if (strokeBrush.Color == backColor)
                    {
                        shape.SetBinding(System.Windows.Shapes.Shape.StrokeProperty, backBinding);
                    }
                }
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(ui); i++)
            {
                var childVisual = (Visual)VisualTreeHelper.GetChild(ui, i);
                SetBinding(childVisual, foreColor, backColor, foreBinding, backBinding);
            }

        }

        public async Task<UIElement> FromSvgStreamAsync(Stream svg)
        {
            if (svg == null) return null;

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 1024
            };

            using var svgReader = XmlReader.Create(svg, settings);
            try
            {
                await using var s = new MemoryStream();
                using (var w = XmlWriter.Create(s))
                {
                    TransformSvg.Transform(svgReader, w);
                }

                s.Seek(0, SeekOrigin.Begin);

                return await FromXamlStreamAsync(s).ConfigureAwait(false);
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}
