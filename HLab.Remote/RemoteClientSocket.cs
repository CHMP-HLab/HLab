#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public class RemoteClientSocket : IRemoteClient

    {
        TcpClient? _client;
        public bool IsConnected => _client?.Connected??false;
        public event EventHandler<string>? MessageReceived;
        public bool Stopping { get; set; } = false;

        public void Listen()
        {
            Task.Run(ListenThread);
        }

        void ListenThread()
        {
            while (!Stopping)
            {
                while (!(_client?.Connected??false))
                {
                    try
                    {
                        _client = new TcpClient("localhost", 25196); // TODO parameter
                    }
                    catch (SocketException)
                    {
                        // TODO start daemon here
                        Thread.Sleep(5000); // TODO parameter
                    }
                }

                var r = new StreamReader(_client.GetStream());

                while (_client?.Connected??false)
                {
                    try
                    {
                        var msg = r.ReadLine();
                        MessageReceived?.Invoke(this,msg);
                    }
                    catch (SocketException) { }
                    catch (IOException) { }
                }
                MessageReceived?.Invoke(this,"<DaemonMessage><State>Dead</State></DaemonMessage>");
            }
        }

        public async Task<string> SendMessageAsync(string message, CancellationToken token)
        {
            while (!_client.Connected)
            {
                await _client.ConnectAsync("localhost", 25196, token);
            }

            var w = new StreamWriter(_client.GetStream());

            var sb = new StringBuilder(message+'\n');

            await w.WriteAsync(sb,token);

            await w.FlushAsync();

            return "";
        }

        public async Task ConnectAsync(int timeout, CancellationToken token)
        {
            if (_client?.Connected??false) return;
            _client = new TcpClient();
            await _client.ConnectAsync("localhost", 25196, token);
        }
    }
}
