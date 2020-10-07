using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public abstract class ClientBase : IDisposable
    {
        private PipeStream _pipe;
        private PipeDirection _direction;
        private Thread _thread;
        private readonly Action<ClientBase> _disposeAction;
        protected string PipeName;

        protected ClientBase(string pipeName, PipeDirection direction, Action<ClientBase> disposeAction=null)
        {
            _direction = direction;
            _disposeAction = disposeAction;
            PipeName = pipeName;
        }

        public async Task SendMessageAsync(string message)
        {
            if (_pipe.IsConnected)
            {
                await using var sw = new StreamWriter(_pipe);
                await sw.WriteAsync(message + '\n');

            }
        }

        public abstract void Start();

        protected void Start(PipeStream pipe)
        {
            _pipe = pipe;
            _thread = new Thread(ProcessClientThread);
            _thread.Start();
        }

        public abstract Task ProcessMessageAsync(string message);

        protected void ProcessClientThread()
        {
            var message = "";


            try
            {
                while (true)
                {
                    Debug.WriteLine("Start listening");
                    var c = _pipe.ReadByte();
                    if (c >= 0)
                    {
                        if (c == '\n') // " "
                        {
                            Debug.WriteLine(message);
                            ProcessMessageAsync(message);
                            message = "";
                        }
                        else message += (char)c;
                    }
                    else break;
                }
            }
            catch (System.IO.IOException)
            {

            }
            finally
            {
                Debug.WriteLine("Close listening");
                Dispose();
                _pipe.Close();
            }
        }

        private bool _disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _disposeAction?.Invoke(this);
                // Dispose managed state (managed objects).
                _pipe?.Dispose();
            }

            _disposed = true;
        }
    }
}
