using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using HLab.Core;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons
{
    public class IconService : Service, IIconService
    {
        private readonly ConcurrentDictionary<string, IIconProvider> _cache = new();
        public object GetIcon(string path, object foreground = null, Size size = default)
        {
            if (path == null) return null;
            object result = null;
            var paths = path.Split('|');
            
            foreach (var p in paths)
            {
                var icon = GetSingleIcon(p, size);
                if (result == null)
                {
                    result = icon; 
                    continue;
                }

                var grid = new IconGrid { MainIcon = { Content = result }, BottomRightIcon = { Content = icon } };
                result = grid;
            }
            return result;
        }

        public async Task<object> GetIconAsync(string path, object foreground = null, Size size = default)
        {
            if (path == null) return null;
            object result = null;
            var paths = path.Split('|');
            
            foreach (var p in paths)
            {
                var icon = await GetSingleIconAsync(p, foreground, size).ConfigureAwait(true);
                if (result == null)
                {
                    result = icon; 
                    continue;
                }

                var grid = new IconGrid { MainIcon = { Content = result }, BottomRightIcon = { Content = icon } };
                result = grid;
            }
            return result;
        }
        private object GetSingleIcon(string path, object foreground = null, Size size = default)
        {

            if (string.IsNullOrWhiteSpace(path)) return null;

            if (_cache.TryGetValue(path.ToLower(), out var iconProvider))
            {
                var icon = iconProvider.Get(size);
                return icon;
            }

            if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
            {
                var icon = iconProviderDefault.Get(size);
                return icon;
            }

            Debug.Print("Icon not found : " + path);

            return null;
        }

        private async Task<object> GetSingleIconAsync(string path, object foreground = null, Size size = default)
        {

            if (string.IsNullOrWhiteSpace(path)) return null;

            if (_cache.TryGetValue(path.ToLower(), out var iconProvider))
            {
                var icon = await iconProvider.GetAsync(foreground,size).ConfigureAwait(true);
                return icon;
            }

            if (_cache.TryGetValue("icons/default", out var iconProviderDefault))
            {
                var icon = await iconProviderDefault.GetAsync(size).ConfigureAwait(true);
                return icon;
            }

            Debug.Print("Icon not found : " + path);

            return null;
        }

        public void AddIconProvider(string name, IIconProvider provider)
        {
            _cache.AddOrUpdate(name.ToLower(), n => provider, (n, p) => provider);
        }

        public IIconProvider GetIconProvider(string name)
        {
            throw new System.NotImplementedException();
        }

    }
}
