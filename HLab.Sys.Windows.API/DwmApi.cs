using System;
using System.Runtime.InteropServices;
using static HLab.Sys.Windows.API.WinDef;

namespace HLab.Sys.Windows.API;

public static partial class DwmApi
{
    public enum DwmWindowAttribute : uint
    {
        NCRenderingEnabled = 1,
        NCRenderingPolicy,
        TransitionsForceDisabled,
        AllowNCPaint,
        CaptionButtonBounds,
        NonClientRtlLayout,
        ForceIconicRepresentation,
        Flip3DPolicy,
        ExtendedFrameBounds,
        HasIconicBitmap,
        DisallowPeek,
        ExcludedFromPeek,
        Cloak,
        Cloaked,
        FreezeRepresentation,
        PassiveUpdateMode,
        UseHostBackdropBrush,
        UseImmersiveDarkMode = 20,
        WindowCornerPreference = 33,
        BorderColor,
        CaptionColor,
        TextColor,
        VisibleFrameBorderThickness,
        SystemBackdropType,
        Last
    }

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmGetWindowAttribute(nint hWnd, DwmWindowAttribute dwAttribute, out Rect pvAttribute, int cbAttribute);

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize);


    public enum AccentState
    {
        Disabled = 0,
        EnableGradient = 1,
        TransparentGradient = 2,
        EnableBlurBehind = 3,
        EnableAcrylicBlurBehind = 4,
        Accent5 = 5,
        Accent6 = 6,
        Accent7 = 7,
        Accent8 = 8,
        Accent9 = 9
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }


    public enum WindowCompositionAttribute
    {
        // ...
        AccentPolicy = 19
        // ...
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public nint Data;
        public int SizeOfData;
    }


    [LibraryImport("user32.dll")]
    public static partial int SetWindowCompositionAttribute(nint hwnd, ref WindowCompositionAttributeData data);
}