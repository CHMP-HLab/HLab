﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MouseHooker
{
    public class MouseHookerWinEvent : MouseHooker
    {
        private readonly WinEventDelegate _proc;
        private static IntPtr _hookId = IntPtr.Zero;

        public MouseHookerWinEvent()
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
            UnhookWinEvent(_hookId);
            _hookId = IntPtr.Zero;
        }

        public override bool Hooked() => _hookId != IntPtr.Zero;

        private IntPtr SetHook(WinEventDelegate proc)
        {
                return SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE,
                    IntPtr.Zero, proc, (uint)0/*curProcess.Id*/, (uint)0, WINEVENT_OUTOFCONTEXT);
        }


        private POINT _oldLocation;
        private void HookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //if (hwnd == IntPtr.Zero)
            {

                if (GetCursorPos(out var p))
                {
                    if (
                        _oldLocation.X != p.X
                        || _oldLocation.Y != p.Y
                    )
                    {
                        _oldLocation = p;
                        OnMouseMove(new HookMouseEventArg { Point = p });
                        Debug.WriteLine(p.X + "," + p.Y);
                    }
                }
            }
            //else
            //{
            //    Console.WriteLine("Location changed");
            //}
        }

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnhookWinEvent(IntPtr hWinEventHook);
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
}
