using System;
using System.Collections.Generic;
using System.Threading;

namespace HLab.Base;

public class ConcurrentHashSet<T> : IDisposable
{
    readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    readonly HashSet<T> _hashSet = new HashSet<T>();

    public List<T> ToList()
    {
        _lock.EnterReadLock();
        try
        {
            var l = new List<T>();
            foreach(var i in _hashSet)
                l.Add(i);

            return l;
        }
        finally
        {
            if (_lock.IsReadLockHeld) _lock.ExitReadLock();
        }

    }


    #region Implementation of ICollection<T> ...ish
    public bool Add(T item)
    {
        _lock.EnterWriteLock();
        try
        {

            return _hashSet.Add(item);
        }
        finally
        {
            if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
        }
    }

    public void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _hashSet.Clear();
        }
        finally
        {
            if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
        }
    }

    public bool Contains(T item)
    {
        _lock.EnterReadLock();
        try
        {
            return _hashSet.Contains(item);
        }
        finally
        {
            if (_lock.IsReadLockHeld) _lock.ExitReadLock();
        }
    }

    public bool TryTake(out T item)
    {
        _lock.EnterWriteLock();
        try
        {
            if (_hashSet.Count == 0)
            {
                item = default;
                return false;
            }

            T result = default;
            foreach (var entry in _hashSet)
            {
                result = entry;
                break;
            }

            if (_hashSet.Remove(result))
            {
                item = result;
                return true;
            }

            item = default;
            return false;
        }
        finally
        {
            if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
        }
    }

    public bool Remove(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _hashSet.Remove(item);
        }
        finally
        {
            if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
        }
    }

    public int Count
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _hashSet.Count;
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }
    }
    #endregion

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
            _lock?.Dispose();
        }
    }
    ~ConcurrentHashSet()
    {
        Dispose(false);
    }
    #endregion
}