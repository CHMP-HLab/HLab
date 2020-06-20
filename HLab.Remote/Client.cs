using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public class RemoteClient
    {
        protected async Task SendAsync([CallerMemberName]string name = null)
        {
            using (var pipeClient = new NamedPipeClientStream("localhost","lbm", PipeDirection.InOut ))
            {
                await pipeClient.ConnectAsync(1000);

                if (pipeClient.IsConnected)
                {
                    using (var sw = new StreamWriter(pipeClient))
                    {
                        await sw.WriteAsync(name);
                    }
                }
            }
        }
        protected async Task<TReturn> SendAsync<TReturn>([CallerMemberName]string name = null)
        {
            var retry = true;
            while (retry)
            {
                retry = false;
                try
                {
                    using (var pipeClient = new NamedPipeClientStream(".", "lbm", PipeDirection.InOut))
                    {
                        await pipeClient.ConnectAsync(1000);

                        if (pipeClient.IsConnected)
                        {
                            using (var sw = new StreamWriter(pipeClient))
                            {
                                await sw.WriteAsync(name+";");
                            }
                        }
                    }

                }
                catch (System.TimeoutException)
                {
                    retry = !StartServer();
                }
            }

            return default;

        }

        protected virtual bool StartServer() => false;
    }
}
