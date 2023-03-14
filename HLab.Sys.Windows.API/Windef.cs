using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API;

public static partial class WinDef
{   
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

    }

    //public struct PointL
    //{
    //    public int X;
    //    public int Y;
    //}

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int X
        {
            get => Left;
            set { Right -= (Left - value); Left = value; }
        }

        public int Y
        {
            get => Top;
            set { Bottom -= (Top - value); Top = value; }
        }

        public int Height
        {
            get => Bottom - Top;
            set => Bottom = value + Top;
        }

        public int Width
        {
            get => Right - Left;
            set => Right = value + Left;
        }

        public static bool operator ==(Rect r1, Rect r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(Rect r1, Rect r2)
        {
            return !r1.Equals(r2);
        }

        public bool Equals(Rect r)
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Rect rect => Equals(rect),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Top, Right, Bottom);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
        }
    }

    public enum DpiAwareness
    {
        Invalid = -1,
        Unaware = 0,
        SystemAware = 1,
        PerMonitorAware = 2
    }


    public enum DpiAwarenessContext
    {
        Unset = 0,
        Unaware = 16,
        SystemAware = 17,
        PerMonitorAware = 18,
        PerMonitorAwareV2 = 34,
    }
}