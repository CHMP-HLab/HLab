using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Size = System.Drawing.Size;

namespace HLab.Icons.Wpf.Icons.Providers
{
    public abstract class IconProvider
    {

        readonly ConcurrentQueue<object> _pool = new();
        public async Task<object> GetAsync()
        {
            while (_pool.TryDequeue(out var pooledIcon))
            {
                if (pooledIcon is FrameworkElement { Parent: null })
                    return pooledIcon;
            }

            var icon = await GetActualAsync();
            return icon;
        }


        public abstract object Get();
        protected abstract Task<object> GetActualAsync();
        public abstract Task<string> GetTemplateAsync();
    }
}