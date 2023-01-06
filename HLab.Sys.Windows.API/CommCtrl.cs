using System.Runtime.InteropServices;
using static HLab.Sys.Windows.API.WinDef;

namespace HLab.Sys.Windows.API;

public static partial class CommCtrl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ImageInfo
    {
        public nint hbmImage;
        public nint hbmMask;
        public int Unused1;
        public int Unused2;
        public Rect rcImage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageListDrawParams
    {
        public int cbSize;
        public nint himl;
        public int i;
        public nint hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;    // x offest from the upperleft of bitmap
        public int yBitmap;    // y offset from the upperleft of bitmap
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }

}