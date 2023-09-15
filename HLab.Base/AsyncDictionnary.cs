using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Base;

public class AsyncDictionary<TKey,T> : IDisposable
{
    readonly SemaphoreSlim _semaphore = new(1);
    readonly ConcurrentDictionary<TKey,T> _cache = new();

    public async Task<T> GetOrAddAsync(TKey key, Func<TKey, Task<T>> factory)
    {
        if(factory==null) throw new ArgumentNullException(nameof(factory));
        if(key==null) return default(T);

        if (_cache.TryGetValue(key, out var result)) return result;
        await _semaphore.WaitAsync().ConfigureAwait(false);    
        try 
        {
            if (_cache.TryGetValue(key, out result)) return result;
            return _cache[key] = await factory(key).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    public T GetOrAdd(TKey key, Func<TKey, T> factory)
    {
        if(factory==null) throw new ArgumentNullException(nameof(factory));

        if (_cache.TryGetValue(key, out var result)) return result;
        _semaphore.Wait();    
        try 
        {
            if (_cache.TryGetValue(key, out result)) return result;
            return _cache[key] = factory(key);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Tuple<bool,T>> TryRemoveAsync(TKey key)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);    
        try
        {
            return _cache.TryRemove(key, out var value) 
                ? Tuple.Create(true,value) 
                : Tuple.Create(false,default(T));
        }
        finally
        {
            _semaphore.Release();
        }
    }
    public IEnumerable<T> Where(Func<T, bool> where)
    {
        _semaphore.Wait();    
        try
        {
            foreach (var item in _cache.Values.Where(where)) yield return item;
            //return await Task.Run(()=>_cache.Values.Where(where).ToList()).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async IAsyncEnumerable<T> WhereAsync(Func<T, bool> where)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);    
        try
        {
            foreach (var item in _cache.Values.Where(where)) yield return item;
            //return await Task.Run(()=>_cache.Values.Where(where).ToList()).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    public IEnumerable<T> Where(Expression<Func<T, bool>> expression)
    {
        var where = expression.Compile();
        return Where(where);
    }

    public IAsyncEnumerable<T> WhereAsync(Expression<Func<T, bool>> expression)
    {
        var where = expression.Compile();
        return WhereAsync(where);
    }

    #region Dispose
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing) 
        {
            // free managed resources
            _semaphore.Dispose();
        }
        // free native resources here if there are any
    }
    ~AsyncDictionary() 
    {
        // Finalizer calls Dispose(false)
        Dispose(false);
    }
    #endregion
}