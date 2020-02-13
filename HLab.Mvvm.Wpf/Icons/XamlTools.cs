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
using System.Xml;
using System.Xml.Xsl;

namespace HLab.Mvvm.Icons
{
    public static class XamlTools
    {
        static Color _foreColor = Colors.Black;
        static Color _backColor = Colors.White;



        private static XslCompiledTransform _transformSvg;
        public static XslCompiledTransform TransformSvg
        {
            get
            {
                if (_transformSvg == null)
                {
                    using var xslStream = Assembly.GetAssembly(typeof(IconProviderSvg))
                        .GetManifestResourceStream("HLab.Mvvm.Icons.svg2xaml.xsl");
                    if (xslStream == null) throw new IOException("xsl file not found");

                    using var stylesheet = XmlReader.Create(xslStream);
                    var settings = new XsltSettings(true,true);
                    _transformSvg = new XslCompiledTransform();
                    _transformSvg.Load(stylesheet, settings, new XmlUrlResolver());
                }
                return _transformSvg;
            }
        }

        public static async Task<string> SvgToXamlAsync(string svg)
        {
            if (svg == null) return null;
            var byteArray = Encoding.ASCII.GetBytes(svg);
            await using var stream = new MemoryStream(byteArray);

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                MaxCharactersFromEntities = 1024
            };

            using var svgReader = XmlReader.Create(stream, settings);
            try
            {
                await using var s = new StringWriter();
                using (var w = XmlWriter.Create(s,TransformSvg.OutputSettings))
                {
                    TransformSvg.Transform(svgReader, w);
                }

                //s.Seek(0, SeekOrigin.Begin);

                return s.ToString();
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

        public static async Task<object> FromSvgStringAsync(string svg)
        {
            if (svg == null) return null;
            var byteArray = Encoding.ASCII.GetBytes(svg);
            await using var stream = new MemoryStream(byteArray);
            return await FromSvgStreamAsync(stream).ConfigureAwait(false);
        }

        public static async Task<UIElement> FromSvgStreamAsync(Stream svg)
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

        public static async Task<UIElement> FromXamlStreamAsync(Stream xamlStream)
        {

            try
            {
                var tcs=new TaskCompletionSource<UIElement>();
                var xr = new XamlReader();

                object obj = null;
                
                xr.LoadCompleted += (o, e) =>
                {
                    if (!(obj is UIElement icon)) return;
                    tcs.SetResult(icon);
                    SetBinding(icon,_foreColor,_backColor);
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

        public static void SetBinding(DependencyObject ui) => SetBinding(ui, _foreColor, _backColor);

        public static void SetBinding(DependencyObject ui, Color foreColor, Color backColor)
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



        private static XslCompiledTransform _transformHtml;

        private static XslCompiledTransform TransformHtml
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
                            var settings = new XsltSettings { EnableDocumentFunction = true };
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
    }
}