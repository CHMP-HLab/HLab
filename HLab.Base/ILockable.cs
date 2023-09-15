using Nito.AsyncEx;

namespace HLab.Base;

public interface ILockable
{
    AsyncReaderWriterLock Lock { get; }
}