using System;
using System.Threading;

namespace HLab.Base;

public interface IUpgradableLockToken : IDisposable
{
    void Write();
}

public class Locker
{
    readonly ReaderWriterLockSlim _lock;
    public Locker(bool recursion = false)
    {
        _lock = new ReaderWriterLockSlim(recursion?LockRecursionPolicy.SupportsRecursion:LockRecursionPolicy.NoRecursion);
    }
    public IDisposable Read => new ReadLockToken(_lock);
    public IDisposable Write => new WriteLockToken(_lock);
    public IUpgradableLockToken ReadUpgradable => new UpgradableLockToken(_lock);
        
//        public object SyncRoot => _lock.

    sealed class ReadLockToken : IDisposable
    {
        ReaderWriterLockSlim _sync;
        public ReadLockToken(ReaderWriterLockSlim sync)
        {
            _sync = sync;
            sync.EnterReadLock();
        }
        public void Dispose()
        {
            if (_sync == null) return;
            _sync.ExitReadLock();
            _sync = null;
        }
    }

    sealed class WriteLockToken : IDisposable
    {
        ReaderWriterLockSlim _sync;
        public WriteLockToken(ReaderWriterLockSlim sync)
        {
            _sync = sync;
            sync.EnterWriteLock();
        }
        public void Dispose()
        {
            if (_sync == null) return;
            _sync.ExitWriteLock();
            _sync = null;
        }
    }

    sealed class UpgradableLockToken : IDisposable, IUpgradableLockToken
    {
        ReaderWriterLockSlim _sync;
        public UpgradableLockToken(ReaderWriterLockSlim sync)
        {
            _sync = sync;
            sync.EnterUpgradeableReadLock();
        }

        public void Write()
        {
            _sync.EnterWriteLock();
        }

        public void Dispose()
        {
            if (_sync == null) return;
            if(_sync.IsWriteLockHeld)
                _sync.ExitWriteLock();
            else
            {
                _sync.ExitUpgradeableReadLock();
            }
            _sync.Dispose();
            _sync = null;
        }
    }

}