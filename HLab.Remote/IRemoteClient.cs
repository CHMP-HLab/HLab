using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public interface IRemoteClient
    {
        Task SendMessageAsync(string message, CancellationToken token);
    }
}