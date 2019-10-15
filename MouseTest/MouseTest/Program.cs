using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MouseHooker;

namespace MouseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var hook = new Hooker();

            hook.MouseMove += Hook_MouseMove;

            hook.Hook();
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
            }
        }

        private static void Hook_MouseMove(object sender, HookMouseEventArg e)
        {
            Console.WriteLine(e.Point.X.ToString(CultureInfo.InvariantCulture) + "," + e.Point.Y.ToString(CultureInfo.InvariantCulture));
        }
    }
}
