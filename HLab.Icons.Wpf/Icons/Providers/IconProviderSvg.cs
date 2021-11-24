using System;
using System.IO;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HLab.Icons.Annotations.Icons;
using Svg;
using Image = System.Windows.Controls.Image;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public class IconProviderSvg : IconProvider, IIconProvider
    {
        private readonly ResourceManager _resourceManager;
        private readonly string _name;
        private readonly Color? _foreColor;
        private bool _parsed = false;
        private string _sourceXaml;

        public IconProviderSvg(ResourceManager resourceManager, string name, Color? foreColor)
        {
            _resourceManager = resourceManager; 
            _name = name;
            _foreColor = foreColor;
        }



        protected override async Task<object> GetAsync(object foreground = null)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            object icon;
            if (_parsed)
            {
                icon = await XamlTools.FromXamlStringAsync(_sourceXaml).ConfigureAwait(true);
                //XamlTools.SetBinding((DependencyObject)icon1, _foreColor);
            }
            else
            {
                AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
                await using var svg = _resourceManager.GetStream(_name);
                icon = await XamlTools.FromSvgStreamAsync(svg).ConfigureAwait(true);

                if (icon != null)
                {
                    _sourceXaml = XamlWriter.Save(icon);
                    _parsed = true;
                 }
            }

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.Value, brush);

            return icon;
        }

        protected override object Get(object foreground = null)
        {
            if (string.IsNullOrWhiteSpace(_name)) return null;

            object icon;
            if (_parsed)
            {
                icon = XamlTools.FromXamlString(_sourceXaml);
            }
            else
            {
                AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
                using var svg = _resourceManager.GetStream(_name);
                icon = XamlTools.FromSvgStream(svg);
                _sourceXaml = XamlWriter.Save(icon);
                _parsed = true;
            }

            if(icon is DependencyObject o && _foreColor.HasValue && foreground is Brush brush)
                XamlTools.SetForeground(o, _foreColor.Value, brush);

            return icon;
        }

        public async Task<object> GetBitmapAsync()
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
            await using var svg = _resourceManager.GetStream(_name);
//            var icon = await XamlTools.FromSvgStreamAsync(svg).ConfigureAwait(true);
            var icon = SvgDocument.Open<SvgDocument>(svg);

            var b = new System.Drawing.Bitmap(128, 128);
            b.MakeTransparent();
            icon.Draw(b);
            return new Image {Source = Convert(b)};
        }

        private BitmapImage Convert(System.Drawing.Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
