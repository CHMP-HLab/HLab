using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{

    public class RemoteEventArgs : EventArgs
    {
        public RemoteEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class RemoteServer
    {
        public void Run()
        {
            _running = true;
            _runningThread = new Thread(ServerLoop);
            _runningThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _terminateHandle.WaitOne();
        }

        public void SendMessage(string message)
        {
            IList<Client> clients;
            lock (_lockClients)
            {
                clients = _clients.ToArray();
            }
            foreach (var client in clients)
                client.SendMessageAsync(message);
        }


        bool _running;
        Thread _runningThread;
        readonly EventWaitHandle _terminateHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        protected string PipeName;

        public event EventHandler<RemoteEventArgs> GotMessage;

        private async Task OnGotMessage(RemoteEventArgs args)
        {
            GotMessage?.Invoke(this, args);
        }
        private async Task OnGotMessage(string message)
        {
            OnGotMessage(new RemoteEventArgs(message));
        }

        private void ServerLoop()
        {
            while (_running)
            {
                ProcessNextClient();
            }

            _terminateHandle.Set();
        }



        private void ProcessNextClient()
        {
            var client = new Client(this,(e)=>
            {
                lock(_lockClients)
                    _clients.Remove((Client) e);
            });
            client.Start();

            lock (_lockClients)
                _clients.Add(client);

        }

        private readonly List<Client> _clients = new List<Client>();
        private readonly object _lockClients = new object();

        public RemoteServer(string pipeName)
        {
            PipeName = pipeName;
        }

        private class Client : ClientBase
        {
            public Client(RemoteServer server,Action<ClientBase> dispose):base(server.PipeName,dispose)
            {
                
                _server = server;
            }

            public override void Start()
            {

                try
                {
                    var pipe = new NamedPipeServerStream(_server.PipeName, PipeDirection.InOut, 254);
                    pipe.WaitForConnection();

                    Start(pipe);
                }
                catch (Exception e)
                {
                    //If there are no more avail connections (254 is in use already) then just keep looping until one is avail
                }
            }

            public override async Task ProcessMessageAsync(string message) => await _server.OnGotMessage(message);

            private readonly RemoteServer _server;
        }
    }
}