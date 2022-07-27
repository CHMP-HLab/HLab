using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using Size = System.Drawing.Size;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public abstract class IconProvider
    {
        readonly ConcurrentDictionary<Size, object> _cache = new();
        public object Get(object background, Size size = default)
        {
            if (size == default) return Get(background);
            if (_cache.TryGetValue(size, out var icon)) return icon;

            var newIcon = Get(background);

            return _cache.GetOrAdd(size,  s => XamlTools.GetBitmap((UIElement)newIcon,s));
            //return _cache.GetOrAdd(size,  s => (UIElement)newIcon);
        }

        public async Task<object> GetAsync(object background, Size size = default)
        {
            if (size == default) return await GetAsync(background);
            if (_cache.TryGetValue(size, out var icon)) return icon;

            var newIcon = await GetAsync(background);

            return _cache.GetOrAdd(size,  s => XamlTools.GetBitmap((UIElement)newIcon,s));
            //return _cache.GetOrAdd(size,  s => (UIElement)newIcon);
        }

        protected abstract object Get(object background = null);
        protected abstract Task<object> GetAsync(object background = null);
    }
}