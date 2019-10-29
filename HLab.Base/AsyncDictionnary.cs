using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Base
{
    public class AsyncDictionary<TKey,T> : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private readonly ConcurrentDictionary<TKey,T> _cache = new ConcurrentDictionary<TKey,T>();

        public async Task<T> GetOrAdd(TKey key, Func<object, Task<T>> factory)
        {
            if(factory==null) throw new ArgumentNullException(nameof(factory));

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

        public async Task<Tuple<bool,T>> TryRemove(TKey key)
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

        public async Task<List<T>> Where(Func<T, bool> where)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);    
            try
            {
                return await Task.Run(()=>_cache.Values.Where(where).ToList()).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<List<T>> Where(Expression<Func<T, bool>> expression)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);    
            try
            {
                return await Task.Run(()=>
                {
                    var e = expression.Compile();
                    return _cache.Values.Where(e).ToList();
                }).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
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
}
