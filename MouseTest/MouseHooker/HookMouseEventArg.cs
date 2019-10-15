using System.Windows;

namespace MouseHooker
{
    public class HookMouseEventArg
    {
        public Point Point { get; set; }
        public bool Handled { get; set; }
    }
}