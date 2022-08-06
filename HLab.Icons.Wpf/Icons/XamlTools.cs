using System;
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
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Xsl;

using HLab.Icons.Wpf.Icons.Providers;

namespace HLab.Icons.Wpf.Icons
{
    public static class XamlTools
    {
        static readonly Color ForeColor = Colors.Yellow;
        static readonly Color BackColor = Colors.Transparent;

        static readonly Brush DefaultForeColor = new SolidColorBrush(ForeColor);
        static readonly Brush DefaultBackColor = new SolidColorBrush(BackColor);

        static XslCompiledTransform _transformSvg;
        public static XslCompiledTransform TransformSvg
        {
            get
            {
                if (_transformSvg != null) return _transformSvg;

                using var xslStream = Assembly.GetAssembly(typeof(IconProviderSvg))
                    .GetManifestResourceStream("HLab.Icons.Wpf.Icons.svg2xaml.xsl");
                if (xslStream == null) throw new IOException("xsl file not found");

                using var stylesheet = XmlReader.Create(xslStream);
                var settings = new XsltSettings(true, true);

                _transformSvg = new XslCompiledTransform();

                XmlUrlResolver resolver = new();

                _transformSvg.Load(stylesheet, settings, resolver);


                return _transformSvg;
            }
        }


        public static async Task<string> SvgToXamlAsync(string svg)
        {
            if (string.IsNullOrEmpty(svg)) return null;
            try
            {
                var byteArray = Encoding.ASCII.GetBytes(svg);
                await using var stream = new MemoryStream(byteArray);

                await using var s = await StreamFromSvgStreamAsync(stream);
                var r = new StreamReader(s);
                var xaml = await r.ReadToEndAsync();

                return xaml;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<object> FromSvgStringAsync(string svg)
        {
            if (svg == null) return null;
            var byteArray = Encoding.ASCII.GetBytes(svg);
            await using var stream = new MemoryStream(byteArray);
            return await FromSvgStreamAsync(stream).ConfigureAwait(false);
        }
        public static object FromSvgString(string svg)
        {
            if (svg == null) return null;
            var byteArray = Encoding.ASCII.GetBytes(svg);
            using var stream = new MemoryStream(byteArray);
            return FromSvgStream(stream);
        }

        public static async Task<UIElement> FromSvgStreamAsync(Stream svg)
        {
            if (svg == null) return null;

            var input = await StreamFromSvgStreamAsync(svg);

            return await FromXamlStreamAsync(input).ConfigureAwait(false);
        }
        public static UIElement FromSvgStream(Stream svg)
        {
            if (svg == null) return null;

            var input = StreamFromSvgStream(svg);

            return FromXamlStream(input);
        }

        static async Task<Stream> StreamFromSvgStreamAsync(Stream svgStream)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 1024
            };

            var writerSettings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize
            };

            using var svgReader = XmlReader.Create(svgStream, settings);
            try
            {
                var outputStream = new MemoryStream();
                await using var xamlWriter = XmlWriter.Create(outputStream, writerSettings);

                TransformSvg.Transform(svgReader, xamlWriter);

                outputStream.Seek(0, SeekOrigin.Begin);

                return outputStream;
            }
            catch (XmlException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }

        }

        static Stream StreamFromSvgStream(Stream svgStream)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 1024
            };

            var writerSettings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize
            };

            using var svgReader = XmlReader.Create(svgStream, settings);
            try
            {
                var outputStream = new MemoryStream();
                using var xamlWriter = XmlWriter.Create(outputStream, writerSettings);

                TransformSvg.Transform(svgReader, xamlWriter);

                outputStream.Seek(0, SeekOrigin.Begin);

                return outputStream;
            }
            catch (XmlException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }

        }

        public static async Task<object> FromXamlStringAsync(string xaml)
        {
            if (xaml == null) return null;
            var byteArray = Encoding.ASCII.GetBytes(xaml);
            await using var stream = new MemoryStream(byteArray);
            return await FromXamlStreamAsync(stream).ConfigureAwait(false);
        }
        public static object FromXamlString(string xaml)
        {
            if (xaml == null) return null;
            var byteArray = Encoding.ASCII.GetBytes(xaml);
            using var stream = new MemoryStream(byteArray);
            return FromXamlStream(stream);
        }

        public static async Task<UIElement> FromXamlStreamAsync(Stream xamlStream)
        {
            try
            {
                var tcs = new TaskCompletionSource<UIElement>();
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 1024
                };

                var xr = new XamlReader();


                object obj = null;

                xr.LoadCompleted += (o, e) =>
                {
                    if (obj is not UIElement icon) return;
                    tcs.SetResult(icon);
                };

                obj = xr.LoadAsync(xamlStream);

                return await tcs.Task.ConfigureAwait(false);
            }
            catch (XamlParseException e)
            {
                return new Viewbox
                {
                    Stretch = Stretch.Uniform,
                    Child = new Canvas
                    {
                        Height = 100,
                        Width = 100,
                        Children = {new Rectangle()
                    {
                        Height = 100, Width = 100,
                        Fill=new SolidColorBrush(Colors.Red)
                    } }
                    },
                    ToolTip = e.Message
                };
            }
            catch (IOException e)
            {
                return new Viewbox
                {
                    Stretch = Stretch.Uniform,
                    Child = new Canvas { Height = 100, Width = 100 },
                    ToolTip = e.Message
                };
            }
        }

        public static UIElement FromXamlStream(Stream xamlStream)
        {
            try
            {
                return (UIElement)XamlReader.Load(xamlStream);
            }
            catch (XamlParseException e)
            {
                return new Viewbox
                {
                    Stretch = Stretch.Uniform,
                    Child = new Canvas
                    {
                        Height = 100,
                        Width = 100,
                        Children = {new Rectangle()
                    {
                        Height = 100, Width = 100,
                        Fill=new SolidColorBrush(Colors.Red)
                    } }
                    },
                    ToolTip = e.Message
                };
            }
            catch (IOException e)
            {
                return new Viewbox
                {
                    Stretch = Stretch.Uniform,
                    Child = new Canvas { Height = 100, Width = 100 },
                    ToolTip = e.Message
                };
            }
        }


        static BindingBase GetForeBinding() => new Binding("Foregound")
            {
//                IsAsync = true,
//                Mode = BindingMode.OneWay,
                FallbackValue = DefaultForeColor,
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                    typeof(Control), 1)
            };

        static BindingBase GetBackBinding() => new Binding("Background")
            {
//                IsAsync = true,
//                Mode = BindingMode.OneWay,
                FallbackValue = DefaultForeColor,
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                    typeof(Control), 1)
            };

        static public DependencyObject SetForeground(DependencyObject ui, Color foregroundColor, Brush foregroundBrush)
        {
            if (ui is Shape shape)
            {
                SetForeground(shape, foregroundColor, foregroundBrush, Shape.FillProperty);
                SetForeground(shape, foregroundColor, foregroundBrush, Shape.StrokeProperty);
            }
            else if (ui is Panel canvas)
            {
                SetForeground(canvas,foregroundColor, foregroundBrush, Panel.BackgroundProperty);
            }
            else if (ui is Viewbox )
            {
            }
            else if (ui is ContainerVisual )
            {
            }
            else
            { }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(ui); i++)
            {
                var childVisual = (Visual)VisualTreeHelper.GetChild(ui, i);
                SetForeground(childVisual, foregroundColor, foregroundBrush);
            }

            return ui;

        }
        static DependencyObject SetForeground(DependencyObject element, Color foregroundColor, Brush foregroundBrush, DependencyProperty property)
        {

                var brush = element.GetValue(property);

                if (brush is SolidColorBrush solidColorBrush)
                {
                    if (solidColorBrush.Color == foregroundColor)
                    {
                        element.SetValue(property,foregroundBrush);
                    }
                }

                return element;
        }

        static public DependencyObject SetBinding_old(DependencyObject ui, Color? foreColor=null, Color? backColor=null)
        {
            if (ui is Shape shape)
            {
                SetBinding(shape,foreColor,backColor, Shape.FillProperty);
                SetBinding(shape,foreColor,backColor, Shape.StrokeProperty);
            }
            else if (ui is Panel canvas)
            {
                SetBinding(canvas,foreColor,backColor, Panel.BackgroundProperty);
            }
            else if (ui is Viewbox )
            {
            }
            else if (ui is ContainerVisual )
            {
            }
            else
            { }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(ui); i++)
            {
                var childVisual = (Visual)VisualTreeHelper.GetChild(ui, i);
                SetBinding_old(childVisual, foreColor, backColor);
            }

            return ui;
        }

        static void SetBinding(FrameworkElement element, Color? foreColor, Color? backColor, DependencyProperty property)
        {
                var brush = element.GetValue(property);

                if (brush is SolidColorBrush solidColorBrush)
                {
                    if (solidColorBrush.Color == foreColor)
                    {
                        //shape.SetValue(Shape.StrokeProperty,Brushes.White);
                        //shape.SetResourceReference(Shape.StrokeProperty, "MahApps.Brushes.ThemeForeground");
                        var b = element.SetBinding(property, GetForeBinding());
                    }
                    else if (solidColorBrush.Color == backColor)
                    {
                        var b = element.SetBinding(property, GetBackBinding());
                    }
                }

        }


        static XslCompiledTransform _transformHtml;

        static XslCompiledTransform TransformHtml
        {
            get
            {
                if (_transformHtml == null)
                {
                    using (var xslStream = Assembly.GetAssembly(typeof(IconService))
                        .GetManifestResourceStream("HLab.Mvvm.Icons.html2xaml.xslt"))
                    {
                        if (xslStream == null) throw new IOException("xsl file not found");
                        using (var stylesheet = XmlReader.Create(xslStream))
                        {
                            var settings = new XsltSettings
                            {

                                EnableDocumentFunction = true
                            };
                            _transformHtml = new XslCompiledTransform();
                            _transformHtml.Load(stylesheet, settings, new XmlUrlResolver());
                        }
                    }
                }
                return _transformHtml;
            }
        }

        public static object GetFromHtml(string html)
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

        public static Image GetBitmap(UIElement source, System.Drawing.Size size)
        {
            var wSize = new Size(size.Height, size.Width);


            var grid = new Grid { Width = size.Width, Height = size.Height };
            var viewbox = new Viewbox
            {
                Width = size.Width,
                Height = size.Height,
                Child = source
            };

            grid.Children.Add(viewbox);

            grid.Measure(wSize);
            grid.Arrange(new Rect(wSize));

            var renderBitmap =
                new RenderTargetBitmap(
                    size.Width,
                    size.Height,
                    96,
                    96,
                    PixelFormats.Pbgra32);
            renderBitmap.Render(grid);
            return new Image { Width = size.Width, Height = size.Height, Source = BitmapFrame.Create(renderBitmap) };
        }

    }
}