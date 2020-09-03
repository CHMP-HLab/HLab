using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public class PipeServer
    {
        bool _running;
        Thread _runningThread;
        readonly EventWaitHandle _terminateHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        public string PipeName { get; set; }
        public Func<string,Task<string>> Command { get; set; }

        private void ServerLoop()
        {
            while (_running)
            {
                ProcessNextClient();
            }

            _terminateHandle.Set();
        }

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

        public virtual async Task<string> ProcessRequest(string message)
        {
            string result = await Command(message);
//            Application.Current.Dispatcher.Invoke(async () => result = await Command(message));

            return result;
        }

        public async void ProcessClientThread(object o)
        {
            await using var pipeStream = (NamedPipeServerStream) o;
            var message = "";
            var running = true;


            try
            {
                while (running)
                {
                    var c = pipeStream.ReadByte();
                    if (c >= 0)
                    {
                        if (c == 32 || c == 59) // " "
                        {
                            var result = await ProcessRequest(message);
                            var bytes = Encoding.ASCII.GetBytes(result);
                            pipeStream.Write(bytes);
                            message = "";

                            if (c == 59) running = false;
                        }
                        else message += (char) c;
                    }
                }

            }
            catch (System.IO.IOException)
            {

            }
            finally
            {
                pipeStream.Close();
            }
        }



        public void ProcessNextClient()
        {
            try
            {
                NamedPipeServerStream pipeStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 254);
                pipeStream.WaitForConnection();

                //Spawn a new thread for each request and continue waiting
                Thread t = new Thread(ProcessClientThread);
                t.Start(pipeStream);
            }
            catch (Exception e)
            {
                //If there are no more avail connections (254 is in use already) then just keep looping until one is avail
            }
        }

    }
}
