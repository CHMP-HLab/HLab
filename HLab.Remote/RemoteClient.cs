using System;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public class RemoteClient
    {
        private readonly string _pipeName;
        public RemoteClient(string pipeName)
        {
            this._pipeName = pipeName;
        }


        public async Task<string> SendMessageAsync(string message)
        {
            var retry = true;
            await using var pipe = new NamedPipeClientStream(
                ".", 
                _pipeName, 
                PipeDirection.InOut, 
                PipeOptions.None);

            

            while (retry)
            {
                try
                {
                    retry = false;
                    await pipe.ConnectAsync(1000).ConfigureAwait(false);
                    if (pipe.IsConnected)
                    {
                        pipe.ReadMode = PipeTransmissionMode.Message;
                        var result = "";
                        await pipe.WriteMessageAsync(message);

                        result = await pipe.ReadMessageAsync();

                        pipe.Close();
                        return result;
                    }
                    else retry = true;
                }
                catch (TimeoutException)
                {
                     retry = await StartServerAsync();
                }

            }

            return "";
        }



        public async Task<string> SendAsync([CallerMemberName]string name = null)
        {
            return await SendMessageAsync(name);
        }


        protected virtual async Task<bool> StartServerAsync() => false;
    }
}
