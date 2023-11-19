using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public class RemoteClientPipe : IRemoteClient
    {
        private readonly string _pipeName;
        public RemoteClientPipe(string pipeName)
        {
            _pipeName = pipeName;
        }

        public async Task SendMessageAsync(string message, CancellationToken token)
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
                    await pipe.ConnectAsync(1000, token).ConfigureAwait(false);
                    if (pipe.IsConnected)
                    {
                        pipe.ReadMode = PipeTransmissionMode.Message;
                        var result = "";
                        await pipe.WriteMessageAsync(message);

                        result = await pipe.ReadMessageAsync();

                        pipe.Close();
                        return;
                    }
                    else retry = true;
                }
                catch (TimeoutException)
                {
                     retry = await StartServerAsync();
                }

            }
        }



       protected virtual async Task<bool> StartServerAsync() => false;
    }
}
