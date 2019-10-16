using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Base
{
    public class AsyncDictionary<TKey,T>
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly Dictionary<TKey,T> _cache = new Dictionary<TKey,T>();

        public async Task<T> GetOrAdd(TKey key, Func<object, Task<T>> factory)
        {
            if (_cache.TryGetValue(key, out var result)) return result;
            await _semaphore.WaitAsync();    
            try 
            {
                if (_cache.TryGetValue(key, out result)) return result;
                return _cache[key] = await factory(key);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Tuple<bool,T>> TryRemove(TKey key)
        {
            await _semaphore.WaitAsync();    
            try
            {
                _cache.Remove(key);
                return _cache.TryGetValue(key, out var value) 
                    ? Tuple.Create(true,value) 
                    : Tuple.Create(false,default(T));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<T>> Where(Func<T, bool> where)
        {
            await _semaphore.WaitAsync();    
            try
            {
                return await Task.Run(()=>_cache.Values.Where(where).ToList());
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<List<T>> Where(Expression<Func<T, bool>> expression)
        {
            await _semaphore.WaitAsync();    
            try
            {
                return await Task.Run(()=>
                {
                    var e = expression.Compile();
                    return _cache.Values.Where(e).ToList();
                });
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
