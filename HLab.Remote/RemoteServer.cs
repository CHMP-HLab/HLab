using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public class RemoteServer
    {
        public void Run()
        {
            _task = ServerLoopAsync();
        }

        public void Stop()
        {
            _running = false;
            _token?.Cancel();
            _task.Wait(1000);
        }

        private Task _task;
        protected string PipeName;

        public event EventHandler<RemoteEventArgs> GotMessage;

        private async Task OnGotMessageAsync(RemoteEventArgs args)
        {
            await Task.Run(()=>GotMessage?.Invoke(this, args)) ;
        }

        private async Task<string> OnGotMessageAsync(string message)
        {
            var arg = new RemoteEventArgs(message);
            await OnGotMessageAsync(arg);
            return arg.Result;
        }


        private CancellationTokenSource _token = null;
        private volatile bool _running = false;

        private async Task ServerLoopAsync()
        {
            _token = new CancellationTokenSource();
            _running = true;
            while (_running)
            {
                try
                {
                    var server = new NamedPipeServerStream(
                        PipeName, 
                        PipeDirection.InOut, 
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Message,
                        PipeOptions.None
                    );


                    await server.WaitForConnectionAsync(_token.Token);

                    if (server.IsConnected)
                    {
                        var msg = await server.ReadMessageAsync();
                        Debug.WriteLine($"got message : {msg}");

                        var response =await OnGotMessageAsync(msg);

                        await server.WriteMessageAsync(response);
                        Debug.WriteLine($"{msg} -> {response}");

                        server.Disconnect();

                        Debug.WriteLine($"{msg} Disconnected.");
                    }
                }
                catch (Exception e)
                {
                    Debug.Write(e);
                    //If there are no more avail connections (254 is in use already) then just keep looping until one is avail
                }
            }
        }


        public RemoteServer(string pipeName)
        {
            PipeName = pipeName;
        }

        private class Client
        {
            //private readonly CancellationToken _token = new CancellationToken();
            public void Abort()
            {
            }

            public Client(RemoteServer server)
            {
                _server = server;
            }



            private readonly RemoteServer _server;
            private bool _disposed = false;

        }
    }
}