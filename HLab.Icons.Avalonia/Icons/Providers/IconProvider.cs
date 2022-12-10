using System.Collections.Concurrent;
using Avalonia;

namespace HLab.Icons.Avalonia.Icons.Providers;

public abstract class IconProvider
{
    public object? Get() => GetPooled()?? GetIcon();
    public async Task<object?> GetAsync() => GetPooled()??await GetIconAsync();

    protected abstract object? GetIcon();
    protected abstract Task<object?> GetIconAsync();
    public abstract Task<string> GetTemplateAsync();


    readonly ConcurrentQueue<object> _pool = new();
    object? GetPooled()
    {
        while (_pool.TryDequeue(out var pooledIcon))
        {
            if (pooledIcon is IStyledElement { Parent: null })
                return pooledIcon;
        }

        return null;
    }
}