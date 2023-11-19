using System.Collections.Concurrent;
using Avalonia;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Avalonia.Icons.Providers;

public abstract class IconProvider : IIconProvider
{
    public object? Get(uint foreground = 0) => GetPooled(foreground)?? GetIcon(foreground);
    public async Task<object?> GetAsync(uint foreground = 0) => GetPooled(foreground)??await GetIconAsync(foreground);

    protected abstract object? GetIcon(uint foreground = 0);
    protected abstract Task<object?> GetIconAsync(uint foreground = 0);
    public abstract Task<string> GetTemplateAsync(uint foreground = 0);


    readonly ConcurrentQueue<object> _pool = new();
    object? GetPooled(uint foreground = 0)
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