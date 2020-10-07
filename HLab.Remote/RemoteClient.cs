using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace HLab.Remote
{
    public class RemoteClient : ClientBase
    {
        public RemoteClient(string pipeName,PipeDirection direction) : base(pipeName)
        {
        }


        public override async void Start()
        {
            var retry = true;
            var pipe = new NamedPipeClientStream("localhost", PipeName, PipeDirection.InOut);
            while(retry)
                try
                {
                    retry = false;
                    await pipe.ConnectAsync(1000);
                }
                catch (TimeoutException)
                {
                     retry = await StartServerAsync();
                }
            if(pipe.IsConnected) Start(pipe);
        }

        public override Task ProcessMessageAsync(string message)
        {
            throw new NotImplementedException();
        }


        public async Task SendAsync([CallerMemberName]string name = null)
        {
            await SendMessageAsync(name);
        }


        protected virtual async Task<bool> StartServerAsync() => false;
    }
}
