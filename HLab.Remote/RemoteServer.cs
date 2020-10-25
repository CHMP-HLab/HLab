using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Text;
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
        public string Result { get; set; } = "ACK";
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

        public async Task SendMessageAsync(string message)
        {
            return;
        }


        private bool _running;
        private Thread _runningThread;
        private readonly EventWaitHandle _terminateHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        protected string PipeName;

        public event EventHandler<RemoteEventArgs> GotMessage;

        private void OnGotMessage(RemoteEventArgs args)
        {
            
            GotMessage?.Invoke(this, args) ;
        }
        private string OnGotMessage(string message)
        {
            var arg = new RemoteEventArgs(message);
            OnGotMessage(arg);
            return arg.Result;
        }

        private void ServerLoop()
        {
            while (_running)
            {
                var client = new Client(this);
                client.Start();
            }

            _terminateHandle.Set();
        }


        public RemoteServer(string pipeName)
        {
            PipeName = pipeName;
        }

        private class Client
        {
            public Client(RemoteServer server)
            {
                _server = server;
            }

            public void Start()
            {
                try
                {
                    var server = new NamedPipeServerStream(
                        _server.PipeName, 
                        PipeDirection.InOut, 
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Message,
                        PipeOptions.None
                        );

                    server.WaitForConnection();
                    if (!server.IsConnected) return;

                    var msg = server.ReadMessage();

                    Debug.WriteLine($"got message : {msg}");

                    var response = ProcessMessage(msg);

                    server.WriteMessage(response);
                    
                    Debug.WriteLine($"{msg} -> {response}");

                    server.Disconnect();
                    Debug.WriteLine($"{msg} Disconnected.");
                }
                catch (Exception e)
                {
                    Debug.Write(e);
                    //If there are no more avail connections (254 is in use already) then just keep looping until one is avail
                }
            }

            private string ProcessMessage(string message) => _server.OnGotMessage(message);

            private readonly RemoteServer _server;
            private bool _disposed = false;

        }
    }
}