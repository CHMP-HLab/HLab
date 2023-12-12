#nullable enable
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote;

public class RemoteClientSocket(string hostname, int port) : IRemoteClient
{
    TcpClient? _client;
    public bool IsConnected => _client?.Connected??false;
    public event EventHandler<string>? MessageReceived;
    public event EventHandler? Connected;
    public event EventHandler? ConnectionFailed;
    public bool Stopping { get; set; } = false;

    public void Listen()
    {
        Task.Run(ListenThread);
    }

    void ListenThread()
    {
        while (!Stopping)
        {
            //if client is not connected, try to connect
            while (_client is null || !_client.Connected)
            {
                var wait = 50;
                while (!Stopping)
                {
                    try
                    {
                        _client = new TcpClient(hostname, port);
                        if (_client.Connected)
                        {
                            Connected?.Invoke(this, EventArgs.Empty);
                            break;
                        }
                    }
                    catch (SocketException)
                    {
                    }

                    try
                    {
                        ConnectionFailed?.Invoke(this, EventArgs.Empty);
                    }
                    catch(Exception ex)
                    {

                    }

                    if (wait>0) Thread.Sleep(wait);
                    if (wait<10000) wait *= 2;
                }
            }

            if (_client is null) continue;

            using (var reader = new StreamReader(_client.GetStream()))
            {
                while (_client.Connected)
                {
                    try
                    {
                        var msg = reader.ReadLine();
                        if (msg != null) MessageReceived?.Invoke(this, msg);
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }

            }

            MessageReceived?.Invoke(this,"<DaemonMessage><State>Dead</State></DaemonMessage>\n");
        }
    }

    async Task<bool> TrySendMessageAsync(string message, CancellationToken token)
    {
        if (_client is null) return false;

        await using var w = new StreamWriter(_client.GetStream());

        var sb = new StringBuilder(message+'\n');

        await w.WriteAsync(sb,token);

#if NET8_0_OR_GREATER
        await w.FlushAsync(token);
#else
            await w.FlushAsync();
#endif
        return true;
    }


    public async Task SendMessageAsync(string message, CancellationToken token)
    {
        var delay = 500;
        for (var i = 0; i < 10; i++)
        {
            try
            {
                if(await TrySendMessageAsync(message, token))
                    return;
            }
            catch (Exception ex)
            {
            }

            await Task.Delay(delay, token);
            delay *= 2;
        }
    }
}