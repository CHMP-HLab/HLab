using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Remote
{
    public static class RemoteExtentions
    {
        public static async Task<string> ReadMessageAsync(this PipeStream stream)
        {
            const int bufferSize = 1024;

            var buff = new byte[bufferSize];
            var mb = new StringBuilder();
            do {
                var byteCount = await stream.ReadAsync(buff,0,bufferSize);
                mb.Append(Encoding.UTF8.GetString(buff, 0, byteCount));
            } while (!(stream.IsMessageComplete));
            return mb.ToString();
        }
        public static async Task WriteMessageAsync(this PipeStream stream, string message)
        {
            var responseBytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length).ConfigureAwait(false);
            await stream.FlushAsync();
            stream.WaitForPipeDrain();
        }
        public static string ReadMessage(this PipeStream stream)
        {
            const int bufferSize = 1024;

            var buff = new byte[bufferSize];
            var mb = new StringBuilder();
            do {
                var byteCount = stream.Read(buff,0,bufferSize);
                mb.Append(Encoding.UTF8.GetString(buff, 0, byteCount));
            } while (!(stream.IsMessageComplete));
            return mb.ToString();
        }
        public static void WriteMessage(this PipeStream stream, string message)
        {
            var responseBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(responseBytes, 0, responseBytes.Length);
            stream.Flush();
            stream.WaitForPipeDrain();
        }
    }
}
