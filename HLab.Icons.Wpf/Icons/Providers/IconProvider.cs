using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows;
using Size = System.Drawing.Size;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public abstract class IconProvider
    {
        private readonly ConcurrentDictionary<Size, object> _cache = new();
        public object Get(Size size)
        {
            if (size == default) return Get();
            if (_cache.TryGetValue(size, out var icon)) return icon;

            var newIcon = Get();

            return _cache.GetOrAdd(size,  s => XamlTools.GetBitmap((UIElement)newIcon,s));
        }

        public async Task<object> GetAsync(Size size)
        {
            if (size == default) return await GetAsync();
            if (_cache.TryGetValue(size, out var icon)) return icon;

            var newIcon = await GetAsync();

            return _cache.GetOrAdd(size,  s => XamlTools.GetBitmap((UIElement)newIcon,s));
        }

        protected abstract object Get();
        protected abstract Task<object> GetAsync();
    }
}