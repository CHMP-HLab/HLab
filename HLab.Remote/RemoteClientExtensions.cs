using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public static class RemoteClientExtensions
    {
        public static Task SendAsync(this IRemoteClient @this, [CallerMemberName] string name = null)
            => @this.SendMessageAsync(name,default);

    }
}