using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace MouseHooker
{
    public class MouseHookerWindowsHook : MouseHooker
    {
        private readonly LowLevelMouseProc _proc;

        private static IntPtr _hookId = IntPtr.Zero;

        public MouseHookerWindowsHook()
        {
            _proc = HookCallback;
        }

        public override void Hook()
        {
            if (_hookId != IntPtr.Zero) UnHook();
            _hookId = SetHook(_proc);

        }

        public override void UnHook()
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        public override bool Hooked() => !(_hookId == IntPtr.Zero);


        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }
        [StructLayout(LayoutKind.Sequential)]

        private struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]

        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);


        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using( var curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private POINT _oldLocation;

        private IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                if (
                    _oldLocation.x != hookStruct.pt.x 
                    || _oldLocation.y != hookStruct.pt.y
                    )
                {
                    _oldLocation = hookStruct.pt;

                    var p = new HookMouseEventArg
                    {
                        Point = new Point(hookStruct.pt.x, hookStruct.pt.y),
                        Handled = false
                    };

                    OnMouseMove(p);

                    if(p.Handled) return new IntPtr(-1);
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
    }
}