using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public class RemoteClient
    {
        public RemoteClient(string pipeName)
        {
            PipeName = pipeName;
        }

        public string PipeName { get; }

        public async Task SendAsync([CallerMemberName]string name = null)
        {
            await using var pipe = new NamedPipeClientStream("localhost", PipeName, PipeDirection.Out);
            if(!pipe.IsConnected)
                await pipe.ConnectAsync(1000);
            if (pipe.IsConnected)
            {
                await using var sw = new StreamWriter(pipe);
                await sw.WriteAsync(name);
            }
        }

        public async Task<TReturn> SendAsync<TReturn>([CallerMemberName]string name = null)
        {
            var retry = true;
            while (retry)
            {
                retry = false;
                try
                {
                    await using var pipe = new NamedPipeClientStream("localhost", PipeName, PipeDirection.InOut);
                    if(!pipe.IsConnected)
                        await pipe.ConnectAsync(1000);
                    if (pipe.IsConnected)
                    {
                        await using var sw = new StreamWriter(pipe);
                        await sw.WriteAsync(name+";");
                    }
                }
                catch (TimeoutException)
                {
                    retry = StartServer();
                }
            }

            return default;

        }


        protected virtual bool StartServer() => false;
    }
}
