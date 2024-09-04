using System;

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
}