using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Icons
{
    [Export(typeof(IIconService)), Singleton]
    public class IconService : IIconService
    {
        private readonly ConcurrentDictionary<string, IIconProvider> _cache = new ConcurrentDictionary<string, IIconProvider>();


        public async Task<object> GetIconAsync(string path)
        {

            if (string.IsNullOrWhiteSpace(path)) return null;

            if (_cache.TryGetValue(path.ToLower(), out var iconProvider))
            {
                return await iconProvider.GetAsync().ConfigureAwait(false);
            }

            if (_cache.TryGetValue("Icons/Default", out var iconProviderDefault))
            {
                return await iconProviderDefault.GetAsync().ConfigureAwait(false);
            }

            Debug.Print("Icon not found : " + path);

            return null;
        }

        public void AddIconProvider(string name, IIconProvider provider)
        {
            _cache.TryAdd(name, provider);
        }

        public async Task<object> GetIconBitmapAsync(string name, System.Drawing.Size size)
        {
            var wSize = new Size(size.Height,size.Width);

            var visual =(UIElement) await GetIconAsync(name).ConfigureAwait(true);

            var grid = new Grid { Width = size.Width, Height = size.Height };
            var viewbox = new Viewbox
            {
                Width = size.Width,
                Height = size.Height,
                Child = visual
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
            return BitmapFrame.Create(renderBitmap);
        }

    }
}
