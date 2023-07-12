using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Media;

namespace HLab.Icons.Avalonia.Icons.Providers;

public abstract class IconProvider
{
    public object? Get(IBrush? foreground) => GetPooled(foreground)?? GetIcon(foreground);
    public async Task<object?> GetAsync(IBrush? foreground) => GetPooled(foreground)??await GetIconAsync(foreground);

    protected abstract object? GetIcon(IBrush? foreground);
    protected abstract Task<object?> GetIconAsync(IBrush? foreground);
    public abstract Task<string> GetTemplateAsync(IBrush? foreground);


    readonly ConcurrentQueue<object> _pool = new();
    object? GetPooled(IBrush? foreground)
    {
        // TODO Urgent : implement foreground
        while (_pool.TryDequeue(out var pooledIcon))
        {
            if (pooledIcon is StyledElement { Parent: null })
                return pooledIcon;
        }

        return null;
    }
}