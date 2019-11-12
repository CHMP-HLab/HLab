using System.Threading;

namespace HLab.Base
{
    public interface ILockable
    {
        ReaderWriterLockSlim Lock { get; }
    }
}