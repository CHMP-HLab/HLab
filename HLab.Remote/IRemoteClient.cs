using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public interface IRemoteClient
    {
        Task<string> SendMessageAsync(string message, CancellationToken token);
    }
}