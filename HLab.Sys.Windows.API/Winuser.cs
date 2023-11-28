using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static HLab.Sys.Windows.API.WinDef;
using static HLab.Sys.Windows.API.WinGdi;

namespace HLab.Sys.Windows.API;

public static partial class WinUser
{
    #region Windows GDI
    public enum DispChange : int
    {
        Successful = 0,
        Restart = 1,
        Failed = -1,
        BadMode = -2,
        NotUpdated = -3,
        BadFlags = -4,
        BadParam = -5,
        BadDualView = -6
    }

    [Flags()]
    public enum ChangeDisplaySettingsFlags : uint
    {
        None = 0,
        UpdateRegistry = 0x00000001,
        Test = 0x00000002,
        Fullscreen = 0x00000004,
        Global = 0x00000008,
        SetPrimary = 0x00000010,
        VideoParameters = 0x00000020,
        EnableUnsafeModes = 0x00000100,
        DisableUnsafeModes = 0x00000200,
        Reset = 0x40000000,
        ResetEx = 0x20000000,
        NoReset = 0x10000000
    }

    [DllImport("user32.dll")]
    public static extern DispChange ChangeDisplaySettings(ref DevMode devMode, ChangeDisplaySettingsFlags flags);

    [LibraryImport("user32.dll")]
    public static partial DispChange ChangeDisplaySettings(nint devMode, ChangeDisplaySettingsFlags flags);

    [DllImport("user32.dll")]
    public static extern DispChange ChangeDisplaySettingsEx(string lpszDeviceName, ref DevMode lpDevMode, nint hwnd, ChangeDisplaySettingsFlags dwflags, nint lParam);

//    public static extern DispChange ChangeDisplaySettingsEx(string lpszDeviceName, ref DevMode lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern DispChange ChangeDisplaySettingsEx(string lpszDeviceName, nint lpDevMode, nint hwnd, ChangeDisplaySettingsFlags dwflags, nint lParam);
    //[LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    //public static partial DispChange ChangeDisplaySettingsEx(string lpszDeviceName, nint lpDevMode, nint hwnd, ChangeDisplaySettingsFlags dwflags, nint lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DevMode devMode);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDisplaySettingsEx(string lpszDeviceName, int iModeNum, ref DevMode lpDevMode, uint dwFlags);


    //[DllImport("user32.dll")]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //public static extern bool EnumDisplaySettingsEx(string deviceName, int modeNum, ref DevMode devMode, int dwFlags);

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        public int Size = (int)Marshal.SizeOf(typeof(MonitorInfoEx));
        public Rect Monitor = default;
        public Rect WorkArea = default;
        public uint Flags = 0;

        public MonitorInfo()
        {
        }
    }


    /// <summary>
    /// The MONITORINFOEX structure contains information about a display monitor.
    /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
    /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name 
    /// for the display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public /*unsafe*/ struct _MonitorInfoEx
    {
        public static MonitorInfoEx Default = new MonitorInfoEx { Size = Unsafe.SizeOf<MonitorInfoEx>() };

        const int CCH_DEVICE_NAME = 32;

        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function. 
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int Size;// (int)Marshal.SizeOf(typeof(MonitorInfoEx));

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. 
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect Monitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications, 
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor. 
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars. 
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect WorkArea;

        /// <summary>
        /// The attributes of the display monitor.
        /// 
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public uint Flags;

        /// <summary>
        /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name, 
        /// and so can save some bytes by using a MONITORINFO structure.
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_DEVICE_NAME)]
        //public fixed char DeviceName[CCH_DEVICE_NAME];

        //public string DeviceNameString
        //{
        //    get
        //    {
        //        var sb = new StringBuilder();
        //            var i = 0;
        //            while (DeviceName[i] != 0)
        //            {
        //                sb.Append(DeviceName[i]);
        //                i++;
        //            }
        //        return sb.ToString();
        //    }
        //}

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_DEVICE_NAME)]
        public string DeviceName;

        public string DeviceNameString => DeviceName;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst=CCH_DEVICE_NAME)] 
        //public char[] DeviceName = new char[CCH_DEVICE_NAME];

        //public MonitorInfoEx() { }
    }


    // size of a device name string
    private const int CCHDEVICENAME = 32;

    /// <summary>
    /// The MONITORINFOEX structure contains information about a display monitor.
    /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
    /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
    /// for the display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MonitorInfoEx
    {
        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int Size;

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect Monitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect WorkArea;

        /// <summary>
        /// The attributes of the display monitor.
        ///
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public uint Flags;

        /// <summary>
        /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name,
        /// and so can save some bytes by using a MONITORINFO structure.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string DeviceName;

        public MonitorInfoEx()
        {
            this.Size = Marshal.SizeOf(typeof(MonitorInfoEx));//72;
            this.DeviceName = string.Empty;
        }
    }




    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public class __MonitorInfoEx
    {
        public int Size = Marshal.SizeOf(typeof(MonitorInfoEx));
        public Rect Monitor = new Rect();
        public Rect WorkArea = new Rect();
        public int Flags = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] DeviceName = new char[32];
    }


    /// <summary>Values to pass to the GetDCEx method.</summary>
    [Flags()]
    public enum DeviceContextValues : uint
    {
        /// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather
        /// than the client rectangle.</summary>
        Window = 0x00000001,
        /// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC
        /// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
        Cache = 0x00000002,
        /// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the
        /// default attributes when this DC is released.</summary>
        NoResetAttrs = 0x00000004,
        /// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows
        /// below the window identified by hWnd.</summary>
        ClipChildren = 0x00000008,
        /// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows
        /// above the window identified by hWnd.</summary>
        ClipSiblings = 0x00000010,
        /// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The
        /// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is
        /// set to the upper-left corner of the window identified by hWnd.</summary>
        ParentClip = 0x00000020,
        /// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded
        /// from the visible region of the returned DC.</summary>
        ExcludeRgn = 0x00000040,
        /// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is
        /// intersected with the visible region of the returned DC.</summary>
        IntersectRgn = 0x00000080,
        /// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
        ExcludeUpdate = 0x00000100,
        /// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
        IntersectUpdate = 0x00000200,
        /// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate
        /// call in effect that would otherwise exclude this window. Used for drawing during
        /// tracking.</summary>
        LockWindowUpdate = 0x00000400,
        /// <summary>DCX_USESTYLE: Undocumented, something related to WM_NCPAINT message.</summary>
        UseStyle = 0x00010000,
        /// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to
        /// be completely validated. Using this function with both DCX_INTERSECTUPDATE and
        /// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
        Validate = 0x00200000,
    }


    [DllImport("user32.dll")]
    public static extern nint GetDCEx(nint hWnd, nint hrgnClip, DeviceContextValues flags);

    [DllImport("user32.dll")]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);


    //Monitor
    //[LibraryImport("user32.dll")]
    //[return: MarshalAs(UnmanagedType.Bool)]
    //public static partial bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMonitorInfo(nint hMonitor, ref MonitorInfo lpmi);





    public delegate bool MonitorEnumProc(nint hMonitor, nint hdcMonitor,
        ref WinDef.Rect lprcMonitor, nint dwData);

    [LibraryImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumDisplayMonitors(
        nint hdc, nint lprcClip, MonitorEnumProc lpfnEnum, nint dwData);

    //1685
    /// <summary>
    /// Provides access to function required to delete handle. This method is used internally
    /// and is not required to be called separately.
    /// </summary>
    /// <param name="hIcon">Pointer to icon handle.</param>
    /// <returns>N/A</returns>

    [LibraryImport("User32.dll")]
    public static partial int DestroyIcon(nint hIcon);

    //1782
    /// <summary>
    /// Retrieves a handle to a window whose class name and window name match the specified strings. The function searches child windows, beginning with the one following the specified child window. This function does not perform a case-sensitive search.
    /// </summary>
    /// <param name="hwndParent">
    /// A handle to the parent window whose child windows are to be searched.
    /// If hwndParent is NULL, the function uses the desktop window as the parent window. The function searches among windows that are child windows of the desktop.
    /// If hwndParent is HWND_MESSAGE, the function searches all message-only windows.
    /// </param>
    /// <param name="hwndChildAfter">
    /// A handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of hwndParent, not just a descendant window.
    /// If hwndChildAfter is NULL, the search begins with the first child window of hwndParent.
    /// Note that if both hwndParent and hwndChildAfter are NULL, the function searches all top-level and message-only windows.
    /// </param>
    /// <param name="lpszClass">
    /// The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.
    /// If lpszClass is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names, or it can be MAKEINTATOM(0x8000). In this latter case, 0x8000 is the atom for a menu class. For more information, see the Remarks section of this topic.
    /// </param>
    /// <param name="lpszWindow">
    /// The window name (the window's title). If this parameter is NULL, all window names match.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is a handle to the window that has the specified class and window names.
    /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint FindWindowEx(nint hwndParent,
        nint hwndChildAfter, string lpszClass, string lpszWindow);

    //1587
    /// <summary>
    /// Confines the cursor to a rectangular area on the screen. If a subsequent cursor position (set by the SetCursorPos function or the mouse) lies outside the rectangle, the system automatically adjusts the position to keep the cursor inside the rectangular area.
    /// </summary>
    /// <param name="lpRect">
    /// <see cref="WinDef.Rect"/>
    /// A pointer to the structure that contains the screen coordinates of the upper-left and lower-right corners of the confining rectangle. If this parameter is NULL, the cursor is free to move anywhere on the screen.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern bool ClipCursor(ref Rect lpRect);

    //1815
    /// <summary>
    /// Retrieves the screen coordinates of the rectangular area to which the cursor is confined.
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclipcursor
    /// </summary>
    /// <param name="lpRect">A pointer to a RECT <see cref="WinDef.Rect"/>  structure that receives the screen coordinates of the confining rectangle. The structure receives the dimensions of the screen if the cursor is not confined to a rectangle.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetClipCursor(out WinDef.Rect lpRect);

    //1833
    /// <summary>
    /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
    /// </summary>
    /// <returns>
    /// The return value is a handle to the desktop window.
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial nint GetDesktopWindow();

    #region SendInput

    [Flags]
    public enum MouseEventF : uint
    {
        Absolute = 0x8000,
        HWheel = 0x01000,
        Move = 0x0001,
        MoveNoCoalesce = 0x2000,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        VirtualDesk = 0x4000,
        Wheel = 0x0800,
        XDown = 0x0080,
        XUp = 0x0100
    }

    [Flags]
    public enum KeyEventF : uint
    {
        None = 0,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        ScanCode = 0x0008,
        Unicode = 0x0004
    }

    public enum VirtualKeyShort : short
    {
        ///<summary>
        ///Left mouse button
        ///</summary>
        LButton = 0x01,
        ///<summary>
        ///Right mouse button
        ///</summary>
        RButton = 0x02,
        ///<summary>
        ///Control-break processing
        ///</summary>
        Cancel = 0x03,
        ///<summary>
        ///Middle mouse button (three-button mouse)
        ///</summary>
        MButton = 0x04,
        ///<summary>
        ///Windows 2000/XP: X1 mouse button
        ///</summary>
        XButton1 = 0x05,
        ///<summary>
        ///Windows 2000/XP: X2 mouse button
        ///</summary>
        XButton2 = 0x06,
        ///<summary>
        ///BACKSPACE key
        ///</summary>
        Back = 0x08,
        ///<summary>
        ///TAB key
        ///</summary>
        Tab = 0x09,
        ///<summary>
        ///CLEAR key
        ///</summary>
        Clear = 0x0C,
        ///<summary>
        ///ENTER key
        ///</summary>
        Return = 0x0D,
        ///<summary>
        ///SHIFT key
        ///</summary>
        Shift = 0x10,
        ///<summary>
        ///CTRL key
        ///</summary>
        Control = 0x11,
        ///<summary>
        ///ALT key
        ///</summary>
        Menu = 0x12,
        ///<summary>
        ///PAUSE key
        ///</summary>
        Pause = 0x13,
        ///<summary>
        ///CAPS LOCK key
        ///</summary>
        Capital = 0x14,
        ///<summary>
        ///Input Method Editor (IME) Kana mode
        ///</summary>
        Kana = 0x15,
        ///<summary>
        ///IME Hangul mode
        ///</summary>
        Hangul = 0x15,
        ///<summary>
        ///IME Junja mode
        ///</summary>
        Junja = 0x17,
        ///<summary>
        ///IME final mode
        ///</summary>
        Final = 0x18,
        ///<summary>
        ///IME Hanja mode
        ///</summary>
        Hanja = 0x19,
        ///<summary>
        ///IME Kanji mode
        ///</summary>
        Kanji = 0x19,
        ///<summary>
        ///ESC key
        ///</summary>
        Escape = 0x1B,
        ///<summary>
        ///IME convert
        ///</summary>
        Convert = 0x1C,
        ///<summary>
        ///IME nonconvert
        ///</summary>
        NonConvert = 0x1D,
        ///<summary>
        ///IME accept
        ///</summary>
        Accept = 0x1E,
        ///<summary>
        ///IME mode change request
        ///</summary>
        ModeChange = 0x1F,
        ///<summary>
        ///SPACEBAR
        ///</summary>
        Space = 0x20,
        ///<summary>
        ///PAGE UP key
        ///</summary>
        Prior = 0x21,
        ///<summary>
        ///PAGE DOWN key
        ///</summary>
        Next = 0x22,
        ///<summary>
        ///END key
        ///</summary>
        End = 0x23,
        ///<summary>
        ///HOME key
        ///</summary>
        Home = 0x24,
        ///<summary>
        ///LEFT ARROW key
        ///</summary>
        Left = 0x25,
        ///<summary>
        ///UP ARROW key
        ///</summary>
        Up = 0x26,
        ///<summary>
        ///RIGHT ARROW key
        ///</summary>
        Right = 0x27,
        ///<summary>
        ///DOWN ARROW key
        ///</summary>
        Down = 0x28,
        ///<summary>
        ///SELECT key
        ///</summary>
        Select = 0x29,
        ///<summary>
        ///PRINT key
        ///</summary>
        Print = 0x2A,
        ///<summary>
        ///EXECUTE key
        ///</summary>
        Execute = 0x2B,
        ///<summary>
        ///PRINT SCREEN key
        ///</summary>
        Snapshot = 0x2C,
        ///<summary>
        ///INS key
        ///</summary>
        Insert = 0x2D,
        ///<summary>
        ///DEL key
        ///</summary>
        Delete = 0x2E,
        ///<summary>
        ///HELP key
        ///</summary>
        Help = 0x2F,
        ///<summary>
        ///0 key
        ///</summary>
        Key0 = 0x30,
        ///<summary>
        ///1 key
        ///</summary>
        Key1 = 0x31,
        ///<summary>
        ///2 key
        ///</summary>
        Key2 = 0x32,
        ///<summary>
        ///3 key
        ///</summary>
        Key3 = 0x33,
        ///<summary>
        ///4 key
        ///</summary>
        Key4 = 0x34,
        ///<summary>
        ///5 key
        ///</summary>
        Key5 = 0x35,
        ///<summary>
        ///6 key
        ///</summary>
        Key6 = 0x36,
        ///<summary>
        ///7 key
        ///</summary>
        Key7 = 0x37,
        ///<summary>
        ///8 key
        ///</summary>
        Key8 = 0x38,
        ///<summary>
        ///9 key
        ///</summary>
        Key9 = 0x39,
        ///<summary>
        ///A key
        ///</summary>
        KeyA = 0x41,
        ///<summary>
        ///B key
        ///</summary>
        KeyB = 0x42,
        ///<summary>
        ///C key
        ///</summary>
        KeyC = 0x43,
        ///<summary>
        ///D key
        ///</summary>
        KeyD = 0x44,
        ///<summary>
        ///E key
        ///</summary>
        KeyE = 0x45,
        ///<summary>
        ///F key
        ///</summary>
        KeyF = 0x46,
        ///<summary>
        ///G key
        ///</summary>
        KeyG = 0x47,
        ///<summary>
        ///H key
        ///</summary>
        KeyH = 0x48,
        ///<summary>
        ///I key
        ///</summary>
        KeyI = 0x49,
        ///<summary>
        ///J key
        ///</summary>
        KeyJ = 0x4A,
        ///<summary>
        ///K key
        ///</summary>
        KeyK = 0x4B,
        ///<summary>
        ///L key
        ///</summary>
        KeyL = 0x4C,
        ///<summary>
        ///M key
        ///</summary>
        KeyM = 0x4D,
        ///<summary>
        ///N key
        ///</summary>
        KeyN = 0x4E,
        ///<summary>
        ///O key
        ///</summary>
        KeyO = 0x4F,
        ///<summary>
        ///P key
        ///</summary>
        KeyP = 0x50,
        ///<summary>
        ///Q key
        ///</summary>
        KeyQ = 0x51,
        ///<summary>
        ///R key
        ///</summary>
        KeyR = 0x52,
        ///<summary>
        ///S key
        ///</summary>
        KeyS = 0x53,
        ///<summary>
        ///T key
        ///</summary>
        KeyT = 0x54,
        ///<summary>
        ///U key
        ///</summary>
        KeyU = 0x55,
        ///<summary>
        ///V key
        ///</summary>
        KeyV = 0x56,
        ///<summary>
        ///W key
        ///</summary>
        KeyW = 0x57,
        ///<summary>
        ///X key
        ///</summary>
        KeyX = 0x58,
        ///<summary>
        ///Y key
        ///</summary>
        KeyY = 0x59,
        ///<summary>
        ///Z key
        ///</summary>
        KeyZ = 0x5A,
        ///<summary>
        ///Left Windows key (Microsoft Natural keyboard) 
        ///</summary>
        LWin = 0x5B,
        ///<summary>
        ///Right Windows key (Natural keyboard)
        ///</summary>
        RWin = 0x5C,
        ///<summary>
        ///Applications key (Natural keyboard)
        ///</summary>
        Apps = 0x5D,
        ///<summary>
        ///Computer Sleep key
        ///</summary>
        Sleep = 0x5F,
        ///<summary>
        ///Numeric keypad 0 key
        ///</summary>
        Numpad0 = 0x60,
        ///<summary>
        ///Numeric keypad 1 key
        ///</summary>
        Numpad1 = 0x61,
        ///<summary>
        ///Numeric keypad 2 key
        ///</summary>
        Numpad2 = 0x62,
        ///<summary>
        ///Numeric keypad 3 key
        ///</summary>
        Numpad3 = 0x63,
        ///<summary>
        ///Numeric keypad 4 key
        ///</summary>
        Numpad4 = 0x64,
        ///<summary>
        ///Numeric keypad 5 key
        ///</summary>
        Numpad5 = 0x65,
        ///<summary>
        ///Numeric keypad 6 key
        ///</summary>
        Numpad6 = 0x66,
        ///<summary>
        ///Numeric keypad 7 key
        ///</summary>
        Numpad7 = 0x67,
        ///<summary>
        ///Numeric keypad 8 key
        ///</summary>
        Numpad8 = 0x68,
        ///<summary>
        ///Numeric keypad 9 key
        ///</summary>
        Numpad9 = 0x69,
        ///<summary>
        ///Multiply key
        ///</summary>
        Multiply = 0x6A,
        ///<summary>
        ///Add key
        ///</summary>
        Add = 0x6B,
        ///<summary>
        ///Separator key
        ///</summary>
        Separator = 0x6C,
        ///<summary>
        ///Subtract key
        ///</summary>
        Substract = 0x6D,
        ///<summary>
        ///Decimal key
        ///</summary>
        Decimal = 0x6E,
        ///<summary>
        ///Divide key
        ///</summary>
        Divide = 0x6F,
        ///<summary>
        ///F1 key
        ///</summary>
        F1 = 0x70,
        ///<summary>
        ///F2 key
        ///</summary>
        F2 = 0x71,
        ///<summary>
        ///F3 key
        ///</summary>
        F3 = 0x72,
        ///<summary>
        ///F4 key
        ///</summary>
        F4 = 0x73,
        ///<summary>
        ///F5 key
        ///</summary>
        F5 = 0x74,
        ///<summary>
        ///F6 key
        ///</summary>
        F6 = 0x75,
        ///<summary>
        ///F7 key
        ///</summary>
        F7 = 0x76,
        ///<summary>
        ///F8 key
        ///</summary>
        F8 = 0x77,
        ///<summary>
        ///F9 key
        ///</summary>
        F9 = 0x78,
        ///<summary>
        ///F10 key
        ///</summary>
        F10 = 0x79,
        ///<summary>
        ///F11 key
        ///</summary>
        F11 = 0x7A,
        ///<summary>
        ///F12 key
        ///</summary>
        F12 = 0x7B,
        ///<summary>
        ///F13 key
        ///</summary>
        F13 = 0x7C,
        ///<summary>
        ///F14 key
        ///</summary>
        F14 = 0x7D,
        ///<summary>
        ///F15 key
        ///</summary>
        F15 = 0x7E,
        ///<summary>
        ///F16 key
        ///</summary>
        F16 = 0x7F,
        ///<summary>
        ///F17 key  
        ///</summary>
        F17 = 0x80,
        ///<summary>
        ///F18 key  
        ///</summary>
        F18 = 0x81,
        ///<summary>
        ///F19 key  
        ///</summary>
        F19 = 0x82,
        ///<summary>
        ///F20 key  
        ///</summary>
        F20 = 0x83,
        ///<summary>
        ///F21 key  
        ///</summary>
        F21 = 0x84,
        ///<summary>
        ///F22 key, (PPC only) Key used to lock device.
        ///</summary>
        F22 = 0x85,
        ///<summary>
        ///F23 key  
        ///</summary>
        F23 = 0x86,
        ///<summary>
        ///F24 key  
        ///</summary>
        F24 = 0x87,
        ///<summary>
        ///NUM LOCK key
        ///</summary>
        NumLock = 0x90,
        ///<summary>
        ///SCROLL LOCK key
        ///</summary>
        Scroll = 0x91,
        ///<summary>
        ///Left SHIFT key
        ///</summary>
        LShift = 0xA0,
        ///<summary>
        ///Right SHIFT key
        ///</summary>
        RShift = 0xA1,
        ///<summary>
        ///Left CONTROL key
        ///</summary>
        LControl = 0xA2,
        ///<summary>
        ///Right CONTROL key
        ///</summary>
        RControl = 0xA3,
        ///<summary>
        ///Left MENU key
        ///</summary>
        LMenu = 0xA4,
        ///<summary>
        ///Right MENU key
        ///</summary>
        RMenu = 0xA5,
        ///<summary>
        ///Windows 2000/XP: Browser Back key
        ///</summary>
        BrowserBack = 0xA6,
        ///<summary>
        ///Windows 2000/XP: Browser Forward key
        ///</summary>
        BrowserForward = 0xA7,
        ///<summary>
        ///Windows 2000/XP: Browser Refresh key
        ///</summary>
        BrowserRefresh = 0xA8,
        ///<summary>
        ///Windows 2000/XP: Browser Stop key
        ///</summary>
        BrowserStop = 0xA9,
        ///<summary>
        ///Windows 2000/XP: Browser Search key 
        ///</summary>
        BrowserSearch = 0xAA,
        ///<summary>
        ///Windows 2000/XP: Browser Favorites key
        ///</summary>
        BrowserFavorites = 0xAB,
        ///<summary>
        ///Windows 2000/XP: Browser Start and Home key
        ///</summary>
        BrowserHome = 0xAC,
        ///<summary>
        ///Windows 2000/XP: Volume Mute key
        ///</summary>
        VolumeMute = 0xAD,
        ///<summary>
        ///Windows 2000/XP: Volume Down key
        ///</summary>
        VolumeDown = 0xAE,
        ///<summary>
        ///Windows 2000/XP: Volume Up key
        ///</summary>
        VolumeUp = 0xAF,
        ///<summary>
        ///Windows 2000/XP: Next Track key
        ///</summary>
        MediaNextTrack = 0xB0,
        ///<summary>
        ///Windows 2000/XP: Previous Track key
        ///</summary>
        MediaPrevTrack = 0xB1,
        ///<summary>
        ///Windows 2000/XP: Stop Media key
        ///</summary>
        MediaStop = 0xB2,
        ///<summary>
        ///Windows 2000/XP: Play/Pause Media key
        ///</summary>
        MediaPlayPause = 0xB3,
        ///<summary>
        ///Windows 2000/XP: Start Mail key
        ///</summary>
        LaunchMail = 0xB4,
        ///<summary>
        ///Windows 2000/XP: Select Media key
        ///</summary>
        LaunchMediaSelect = 0xB5,
        ///<summary>
        ///Windows 2000/XP: Start Application 1 key
        ///</summary>
        LaunchApp1 = 0xB6,
        ///<summary>
        ///Windows 2000/XP: Start Application 2 key
        ///</summary>
        LaunchApp2 = 0xB7,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        Oem1 = 0xBA,
        ///<summary>
        ///Windows 2000/XP: For any country/region, the '+' key
        ///</summary>
        OemPlus = 0xBB,
        ///<summary>
        ///Windows 2000/XP: For any country/region, the ',' key
        ///</summary>
        OemComma = 0xBC,
        ///<summary>
        ///Windows 2000/XP: For any country/region, the '-' key
        ///</summary>
        OemMinus = 0xBD,
        ///<summary>
        ///Windows 2000/XP: For any country/region, the '.' key
        ///</summary>
        OemPeriod = 0xBE,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        Oem2 = 0xBF,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard. 
        ///</summary>
        Oem3 = 0xC0,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard. 
        ///</summary>
        Oem4 = 0xDB,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard. 
        ///</summary>
        Oem5 = 0xDC,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard. 
        ///</summary>
        Oem6 = 0xDD,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard. 
        ///</summary>
        Oem7 = 0xDE,
        ///<summary>
        ///Used for miscellaneous characters; it can vary by keyboard.
        ///</summary>
        Oem8 = 0xDF,
        ///<summary>
        ///Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
        ///</summary>
        Oem102 = 0xE2,
        ///<summary>
        ///Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        ///</summary>
        ProcessKey = 0xE5,
        ///<summary>
        ///Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes.
        ///The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information,
        ///see Remark in KeybdInput, SendInput, WM_KeyDOWN, and WM_KeyUP
        ///</summary>
        Packet = 0xE7,
        ///<summary>
        ///Attn key
        ///</summary>
        Attn = 0xF6,
        ///<summary>
        ///CrSel key
        ///</summary>
        CrSel = 0xF7,
        ///<summary>
        ///ExSel key
        ///</summary>
        ExsSel = 0xF8,
        ///<summary>
        ///Erase EOF key
        ///</summary>
        ErEof = 0xF9,
        ///<summary>
        ///Play key
        ///</summary>
        Play = 0xFA,
        ///<summary>
        ///Zoom key
        ///</summary>
        Zoom = 0xFB,
        ///<summary>
        ///Reserved 
        ///</summary>
        NoName = 0xFC,
        ///<summary>
        ///PA1 key
        ///</summary>
        Pa1 = 0xFD,
        ///<summary>
        ///Clear key
        ///</summary>
        OemClear = 0xFE
    }

    public enum ScanCodeShort : short
    {
        LButton = 0,
        RButton = 0,
        Cancel = 70,
        MButton = 0,
        XButton1 = 0,
        XButton2 = 0,
        Back = 14,
        Tab = 15,
        Clear = 76,
        Return = 28,
        Shift = 42,
        Control = 29,
        Menu = 56,
        Pause = 0,
        Capital = 58,
        Kana = 0,
        Hangul = 0,
        Junja = 0,
        Final = 0,
        Hanja = 0,
        Kanji = 0,
        Escape = 1,
        Convert = 0,
        NonConvert = 0,
        Accept = 0,
        ModeChange = 0,
        Space = 57,
        Prior = 73,
        Next = 81,
        End = 79,
        Home = 71,
        Left = 75,
        Up = 72,
        Right = 77,
        Down = 80,
        Select = 0,
        Print = 0,
        Execute = 0,
        Snapshot = 84,
        Insert = 82,
        Delete = 83,
        Help = 99,
        Key0 = 11,
        Key1 = 2,
        Key2 = 3,
        Key3 = 4,
        Key4 = 5,
        Key5 = 6,
        Key6 = 7,
        Key7 = 8,
        Key8 = 9,
        Key9 = 10,
        KeyA = 30,
        KeyB = 48,
        KeyC = 46,
        KeyD = 32,
        KeyE = 18,
        KeyF = 33,
        KeyG = 34,
        KeyH = 35,
        KeyI = 23,
        KeyJ = 36,
        KeyK = 37,
        KeyL = 38,
        KeyM = 50,
        KeyN = 49,
        KeyO = 24,
        KeyP = 25,
        KeyQ = 16,
        KeyR = 19,
        KeyS = 31,
        KeyT = 20,
        KeyU = 22,
        KeyV = 47,
        KeyW = 17,
        KeyX = 45,
        KeyY = 21,
        KeyZ = 44,
        LWin = 91,
        RWin = 92,
        Apps = 93,
        Sleep = 95,
        Numpad0 = 82,
        Numpad1 = 79,
        Numpad2 = 80,
        Numpad3 = 81,
        Numpad4 = 75,
        Numpad5 = 76,
        Numpad6 = 77,
        Numpad7 = 71,
        Numpad8 = 72,
        Numpad9 = 73,
        Multiply = 55,
        Add = 78,
        Separator = 0,
        Subtract = 74,
        Decimal = 83,
        Divide = 53,
        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,
        F6 = 64,
        F7 = 65,
        F8 = 66,
        F9 = 67,
        F10 = 68,
        F11 = 87,
        F12 = 88,
        F13 = 100,
        F14 = 101,
        F15 = 102,
        F16 = 103,
        F17 = 104,
        F18 = 105,
        F19 = 106,
        F20 = 107,
        F21 = 108,
        F22 = 109,
        F23 = 110,
        F24 = 118,
        NumLock = 69,
        Scroll = 70,
        LShift = 42,
        RShift = 54,
        LControl = 29,
        RControl = 29,
        LMenu = 56,
        RMenu = 56,
        BrowserBack = 106,
        BrowserForward = 105,
        BrowserRefresh = 103,
        BrowserStop = 104,
        BrowserSearch = 101,
        BrowserFavorites = 102,
        BrowserHome = 50,
        VolumeMute = 32,
        VolumeDown = 46,
        VolumeUp = 48,
        MediaNextTrack = 25,
        MediaPrevTrack = 16,
        MediaStop = 36,
        MediaPlayPause = 34,
        LaunchMail = 108,
        LaunchMediaSelect = 109,
        LaunchApp1 = 107,
        LaunchApp2 = 33,
        Oem1 = 39,
        OemPlus = 13,
        OemComma = 51,
        OemMinus = 12,
        OemPeriod = 52,
        Oem2 = 53,
        Oem3 = 41,
        Oem4 = 26,
        Oem5 = 43,
        Oem6 = 27,
        Oem7 = 40,
        Oem8 = 0,
        Oem102 = 86,
        ProcessKey = 0,
        Packet = 0,
        Attn = 0,
        CrSel = 0,
        ExSel = 0,
        ErEof = 93,
        Play = 0,
        Zoom = 98,
        NoName = 0,
        Pa1 = 0,
        OemClear = 0,
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public int dx;
        public int dy;
        public int mouseData;
        public MouseEventF dwFlags;
        public uint time;
        public nint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeybdInput
    {
        public VirtualKeyShort wVk;
        public ScanCodeShort wScan;
        public KeyEventF dwFlags;
        public int time;
        public nuint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public int type;
        [FieldOffset(4)] public MouseInput mouseInput;
        [FieldOffset(4)] public KeybdInput keyboardInput;
        [FieldOffset(4)] public HardwareInput hardwareInput;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
        public uint Type;
        public InputUnion Union;
        public static int Size => Marshal.SizeOf(typeof(Input));
    }

    //2291
    /// <summary>
    /// Synthesizes keystrokes, mouse motions, and button clicks.
    /// </summary>
    /// <param name="nInputs">The number of structures in the pInputs array.</param>
    /// <param name="pInputs">An array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
    /// <param name="cbSize">The size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.</param>
    /// <returns>
    /// The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns zero, the input was already blocked by another thread. To get extended error information, call GetLastError.
    /// This function fails when it is blocked by UIPI. Note that neither GetLastError nor the return value will indicate the failure was caused by UIPI blocking.
    /// </returns>
    /// <remarks>
    /// This function is subject to UIPI. Applications are permitted to inject input only into applications that are at an equal or lesser integrity level.
    /// The SendInput function inserts the events in the INPUT structures serially into the keyboard or mouse input stream. These events are not interspersed with other keyboard or mouse input events inserted either by the user (with the keyboard or mouse) or by calls to keybd_event, mouse_event, or other calls to SendInput.
    /// This function does not reset the keyboard's current state. Any keys that are already pressed when the function is called might interfere with the events that this function generates. To avoid this problem, check the keyboard's state with the GetAsyncKeyState function and correct as necessary.
    /// Because the touch keyboard uses the surrogate macros defined in winnls.h to send input to the system, a listener on the keyboard event hook must decode input originating from the touch keyboard. For more information, see Surrogates and Supplementary Characters.
    /// An accessibility application can use SendInput to inject keystrokes corresponding to application launch shortcut keys that are handled by the shell. This functionality is not guaranteed to work for other types of applications.
    /// </remarks>

    [DllImport("user32.dll")]
    public static extern uint SendInput(uint nInputs,
        [MarshalAs(UnmanagedType.LPArray), In] InputUnion[] pInputs,
        int cbSize);
    #endregion

    //2004
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(nint hWnd, out Rect lpRect);

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        /// </summary>
        AsyncWindowPos = 0x4000,

        /// <summary>
        ///     Prevents generation of the WM_SYNCPAINT message.
        /// </summary>
        DeferErase = 0x2000,

        /// <summary>
        ///     Draws a frame (defined in the window's class description) around the window.
        /// </summary>
        DrawFrame = 0x0020,

        /// <summary>
        ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        /// </summary>
        FrameChanged = 0x0020,

        /// <summary>
        ///     Hides the window.
        /// </summary>
        HideWindow = 0x0080,

        /// <summary>
        ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        /// </summary>
        NoActivate = 0x0010,

        /// <summary>
        ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        /// </summary>
        NoCopyBits = 0x0100,

        /// <summary>
        ///     Retains the current position (ignores X and Y parameters).
        /// </summary>
        NoMove = 0x0002,

        /// <summary>
        ///     Does not change the owner window's position in the Z order.
        /// </summary>
        NoOwnerZOrder = 0x0200,

        /// <summary>
        ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        /// </summary>
        NoRedraw = 0x0008,

        /// <summary>
        ///     Same as the SWP_NOOWNERZORDER flag.
        /// </summary>
        NoReposition = 0x0200,

        /// <summary>
        ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
        /// </summary>
        NoSendChanging = 0x0400,

        /// <summary>
        ///     Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        NoSize = 0x0001,

        /// <summary>
        ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
        /// </summary>
        NoZOrder = 0x0004,

        /// <summary>
        ///     Displays the window.
        /// </summary>
        ShowWindow = 0x0040,

        // ReSharper restore InconsistentNaming
    }

    public enum HandleWindow : int
    {
        TopMost = -1,
        NoTopMost = -2,
        Top = 0,
        Bottom = 1,
    }


    // 2397
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);
    public static bool SetWindowPos(nint hWnd, HandleWindow hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags)
        => SetWindowPos(hWnd, (nint)hWndInsertAfter, x, y, cx, cy, uFlags);

    #region GetWindowPlacement
    public enum ShowWindowCommands : int
    {
        Hide = 0,
        Normal = 1,
        Minimized = 2,
        Maximized = 3,
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public int length = Marshal.SizeOf<WindowPlacement>();
        public int flags;
        public ShowWindowCommands showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public Rect rcNormalPosition;

        public WindowPlacement()
        {
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowPlacement(nint hWnd, ref WindowPlacement lpwndpl);

    #endregion

    //2327
    [DllImport("user32.dll", SetLastError = true)]
    public static extern nint SetFocus(nint hWnd);

    [DllImport("user32.dll")]
    public static extern nint SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern nint SetActiveWindow(nint hWnd);

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(nint hWnd, out int lpdwProcessId);

    public enum ShowWindowEnum
    {
        Hide = 0,
        ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
        Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
        Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
        Restore = 9, ShowDefault = 10, ForceMinimized = 11
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ShowWindow(nint hWnd, ShowWindowEnum flags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsWindowVisible(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumChildWindows(nint hwndParent, EnumWindowsProc lpEnumFunc, nint lParam);

    public delegate bool EnumWindowsProc(nint hWnd, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumDesktopWindows(nint hDesktop, EnumWindowsProc ewp, int lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out WinDef.Point lpPoint);

    [Flags]
    public enum WindowMessage2 : uint
    {
        WM_NULL = 0x0,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000a,
        WM_SETREDRAW = 0x000b,
        WM_SETTEXT = 0x000c,
        WM_GETTEXT = 0x000d,
        WM_GETTEXTLENGTH = 0x000e,
        WM_PAINT = 0x000f,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_ENDSESSION = 0x0016,
        WM_SHOWWINDOW = 0x0018,
        WM_CTLCOLOR = 0x0019,
        WM_WININICHANGE = 0x001a,
        WM_DEVMODECHANGE = 0x001b,
        WM_ACTIVATEAPP = 0x001c,
        WM_FONTCHANGE = 0x001d,
        WM_TIMECHANGE = 0x001e,
        WM_CANCELMODE = 0x001f,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINTICON = 0x0026,
        WM_ICONERASEBKGND = 0x0027,
        WM_NEXTDLGCTL = 0x0028,
        WM_SPOOLERSTATUS = 0x002a,
        WM_DRAWITEM = 0x002b,
        WM_MEASUREITEM = 0x002c,
        WM_DELETEITEM = 0x002d,
        WM_VKEYTOITEM = 0x002e,
        WM_CHARTOITEM = 0x002f,
        WM_SETFONT = 0x0030,
        WM_GETFONT = 0x0031,
        WM_SETHOTKEY = 0x0032,
        WM_GETHOTKEY = 0x0033,
        WM_QUERYDRAGICON = 0x0037,
        WM_COMPAREITEM = 0x0039,
        WM_GETOBJECT = 0x003d,
        WM_COMPACTING = 0x0041,
        WM_COMMNOTIFY = 0x0044,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_POWER = 0x0048,
        WM_COPYGLOBALDATA = 0x0049,
        WM_COPYDATA = 0x004a,
        WM_CANCELJOURNAL = 0x004b,
        WM_NOTIFY = 0x004e,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_TCARD = 0x0052,
        WM_HELP = 0x0053,
        WM_USERCHANGED = 0x0054,
        WM_NOTIFYFORMAT = 0x0055,
        WM_CONTEXTMENU = 0x007b,
        WM_STYLECHANGING = 0x007c,
        WM_STYLECHANGED = 0x007d,
        WM_DISPLAYCHANGE = 0x007e,
        WM_GETICON = 0x007f,
        WM_SETICON = 0x0080,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCCALCSIZE = 0x0083,
        WM_NCHITTEST = 0x0084,
        WM_NCPAINT = 0x0085,
        WM_NCACTIVATE = 0x0086,
        WM_GETDLGCODE = 0x0087,
        WM_SYNCPAINT = 0x0088,
        WM_NCMOUSEMOVE = 0x00a0,
        WM_NCLBUTTONDOWN = 0x00a1,
        WM_NCLBUTTONUP = 0x00a2,
        WM_NCLBUTTONDBLCLK = 0x00a3,
        WM_NCRBUTTONDOWN = 0x00a4,
        WM_NCRBUTTONUP = 0x00a5,
        WM_NCRBUTTONDBLCLK = 0x00a6,
        WM_NCMBUTTONDOWN = 0x00a7,
        WM_NCMBUTTONUP = 0x00a8,
        WM_NCMBUTTONDBLCLK = 0x00a9,
        WM_NCXBUTTONDOWN = 0x00ab,
        WM_NCXBUTTONUP = 0x00ac,
        WM_NCXBUTTONDBLCLK = 0x00ad,
        SBM_SETPOS = 0x00e0,
        SBM_GETPOS = 0x00e1,
        SBM_SETRANGE = 0x00e2,
        SBM_GETRANGE = 0x00e3,
        SBM_ENABLE_ARROWS = 0x00e4,
        SBM_SETRANGEREDRAW = 0x00e6,
        SBM_SETSCROLLINFO = 0x00e9,
        SBM_GETSCROLLINFO = 0x00ea,
        SBM_GETSCROLLBARINFO = 0x00eb,
        WM_INPUT = 0x00ff,
        WM_KEYDOWN = 0x0100,
        WM_KEYFIRST = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_DEADCHAR = 0x0103,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_SYSCHAR = 0x0106,
        WM_SYSDEADCHAR = 0x0107,
        WM_KEYLAST = 0x0108,
        WM_WNT_CONVERTREQUESTEX = 0x0109,
        WM_CONVERTREQUEST = 0x010a,
        WM_CONVERTRESULT = 0x010b,
        WM_INTERIM = 0x010c,
        WM_IME_STARTCOMPOSITION = 0x010d,
        WM_IME_ENDCOMPOSITION = 0x010e,
        WM_IME_COMPOSITION = 0x010f,
        WM_IME_KEYLAST = 0x010f,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111,
        WM_SYSCOMMAND = 0x0112,
        WM_TIMER = 0x0113,
        WM_HSCROLL = 0x0114,
        WM_VSCROLL = 0x0115,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_SYSTIMER = 0x0118,
        WM_MENUSELECT = 0x011f,
        WM_MENUCHAR = 0x0120,
        WM_ENTERIDLE = 0x0121,
        WM_MENURBUTTONUP = 0x0122,
        WM_MENUDRAG = 0x0123,
        WM_MENUGETOBJECT = 0x0124,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_MENUCOMMAND = 0x0126,
        WM_CHANGEUISTATE = 0x0127,
        WM_UPDATEUISTATE = 0x0128,
        WM_QUERYUISTATE = 0x0129,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_MOUSELAST = 0x0209,
        WM_MOUSEWHEEL = 0x020a,
        WM_XBUTTONDOWN = 0x020b,
        WM_XBUTTONUP = 0x020c,
        WM_XBUTTONDBLCLK = 0x020d,
        WM_PARENTNOTIFY = 0x0210,
        WM_ENTERMENULOOP = 0x0211,
        WM_EXITMENULOOP = 0x0212,
        WM_NEXTMENU = 0x0213,
        WM_SIZING = 0x0214,
        WM_CAPTURECHANGED = 0x0215,
        WM_MOVING = 0x0216,
        WM_POWERBROADCAST = 0x0218,
        WM_DEVICECHANGE = 0x0219,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIACTIVATE = 0x0222,
        WM_MDIRESTORE = 0x0223,
        WM_MDINEXT = 0x0224,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDITILE = 0x0226,
        WM_MDICASCADE = 0x0227,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDISETMENU = 0x0230,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_DROPFILES = 0x0233,
        WM_MDIREFRESHMENU = 0x0234,
        WM_IME_REPORT = 0x0280,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_CONTROL = 0x0283,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_SELECT = 0x0285,
        WM_IME_CHAR = 0x0286,
        WM_IME_REQUEST = 0x0288,
        WM_IMEKEYDOWN = 0x0290,
        WM_IME_KEYDOWN = 0x0290,
        WM_IMEKEYUP = 0x0291,
        WM_IME_KEYUP = 0x0291,
        WM_NCMOUSEHOVER = 0x02a0,
        WM_MOUSEHOVER = 0x02a1,
        WM_NCMOUSELEAVE = 0x02a2,
        WM_MOUSELEAVE = 0x02a3,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_RENDERFORMAT = 0x0305,
        WM_RENDERALLFORMATS = 0x0306,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_VSCROLLCLIPBOARD = 0x030a,
        WM_SIZECLIPBOARD = 0x030b,
        WM_ASKCBFORMATNAME = 0x030c,
        WM_CHANGECBCHAIN = 0x030d,
        WM_HSCROLLCLIPBOARD = 0x030e,
        WM_QUERYNEWPALETTE = 0x030f,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PALETTECHANGED = 0x0311,
        WM_HOTKEY = 0x0312,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_APPCOMMAND = 0x0319,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035f,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037f,
        WM_PENWINFIRST = 0x0380,
        WM_RCRESULT = 0x0381,
        WM_HOOKRCRESULT = 0x0382,
        WM_GLOBALRCCHANGE = 0x0383,
        WM_PENMISCINFO = 0x0383,
        WM_SKB = 0x0384,
        WM_HEDITCTL = 0x0385,
        WM_PENCTL = 0x0385,
        WM_PENMISC = 0x0386,
        WM_CTLINIT = 0x0387,
        WM_PENEVENT = 0x0388,
        WM_PENWINLAST = 0x038f,
        DDM_SETFMT = 0x0400,
        DM_GETDEFID = 0x0400,
        NIN_SELECT = 0x0400,
        TBM_GETPOS = 0x0400,
        WM_PSD_PAGESETUPDLG = 0x0400,
        WM_USER = 0x0400,
        CBEM_INSERTITEMA = 0x0401,
        DDM_DRAW = 0x0401,
        DM_SETDEFID = 0x0401,
        HKM_SETHOTKEY = 0x0401,
        PBM_SETRANGE = 0x0401,
        RB_INSERTBANDA = 0x0401,
        SB_SETTEXTA = 0x0401,
        TB_ENABLEBUTTON = 0x0401,
        TBM_GETRANGEMIN = 0x0401,
        TTM_ACTIVATE = 0x0401,
        WM_CHOOSEFONT_GETLOGFONT = 0x0401,
        WM_PSD_FULLPAGERECT = 0x0401,
        CBEM_SETIMAGELIST = 0x0402,
        DDM_CLOSE = 0x0402,
        DM_REPOSITION = 0x0402,
        HKM_GETHOTKEY = 0x0402,
        PBM_SETPOS = 0x0402,
        RB_DELETEBAND = 0x0402,
        SB_GETTEXTA = 0x0402,
        TB_CHECKBUTTON = 0x0402,
        TBM_GETRANGEMAX = 0x0402,
        WM_PSD_MINMARGINRECT = 0x0402,
        CBEM_GETIMAGELIST = 0x0403,
        DDM_BEGIN = 0x0403,
        HKM_SETRULES = 0x0403,
        PBM_DELTAPOS = 0x0403,
        RB_GETBARINFO = 0x0403,
        SB_GETTEXTLENGTHA = 0x0403,
        TBM_GETTIC = 0x0403,
        TB_PRESSBUTTON = 0x0403,
        TTM_SETDELAYTIME = 0x0403,
        WM_PSD_MARGINRECT = 0x0403,
        CBEM_GETITEMA = 0x0404,
        DDM_END = 0x0404,
        PBM_SETSTEP = 0x0404,
        RB_SETBARINFO = 0x0404,
        SB_SETPARTS = 0x0404,
        TB_HIDEBUTTON = 0x0404,
        TBM_SETTIC = 0x0404,
        TTM_ADDTOOLA = 0x0404,
        WM_PSD_GREEKTEXTRECT = 0x0404,
        CBEM_SETITEMA = 0x0405,
        PBM_STEPIT = 0x0405,
        TB_INDETERMINATE = 0x0405,
        TBM_SETPOS = 0x0405,
        TTM_DELTOOLA = 0x0405,
        WM_PSD_ENVSTAMPRECT = 0x0405,
        CBEM_GETCOMBOCONTROL = 0x0406,
        PBM_SETRANGE32 = 0x0406,
        RB_SETBANDINFOA = 0x0406,
        SB_GETPARTS = 0x0406,
        TB_MARKBUTTON = 0x0406,
        TBM_SETRANGE = 0x0406,
        TTM_NEWTOOLRECTA = 0x0406,
        WM_PSD_YAFULLPAGERECT = 0x0406,
        CBEM_GETEDITCONTROL = 0x0407,
        PBM_GETRANGE = 0x0407,
        RB_SETPARENT = 0x0407,
        SB_GETBORDERS = 0x0407,
        TBM_SETRANGEMIN = 0x0407,
        TTM_RELAYEVENT = 0x0407,
        CBEM_SETEXSTYLE = 0x0408,
        PBM_GETPOS = 0x0408,
        RB_HITTEST = 0x0408,
        SB_SETMINHEIGHT = 0x0408,
        TBM_SETRANGEMAX = 0x0408,
        TTM_GETTOOLINFOA = 0x0408,
        CBEM_GETEXSTYLE = 0x0409,
        CBEM_GETEXTENDEDSTYLE = 0x0409,
        PBM_SETBARCOLOR = 0x0409,
        RB_GETRECT = 0x0409,
        SB_SIMPLE = 0x0409,
        TB_ISBUTTONENABLED = 0x0409,
        TBM_CLEARTICS = 0x0409,
        TTM_SETTOOLINFOA = 0x0409,
        CBEM_HASEDITCHANGED = 0x040a,
        RB_INSERTBANDW = 0x040a,
        SB_GETRECT = 0x040a,
        TB_ISBUTTONCHECKED = 0x040a,
        TBM_SETSEL = 0x040a,
        TTM_HITTESTA = 0x040a,
        WIZ_QUERYNUMPAGES = 0x040a,
        CBEM_INSERTITEMW = 0x040b,
        RB_SETBANDINFOW = 0x040b,
        SB_SETTEXTW = 0x040b,
        TB_ISBUTTONPRESSED = 0x040b,
        TBM_SETSELSTART = 0x040b,
        TTM_GETTEXTA = 0x040b,
        WIZ_NEXT = 0x040b,
        CBEM_SETITEMW = 0x040c,
        RB_GETBANDCOUNT = 0x040c,
        SB_GETTEXTLENGTHW = 0x040c,
        TB_ISBUTTONHIDDEN = 0x040c,
        TBM_SETSELEND = 0x040c,
        TTM_UPDATETIPTEXTA = 0x040c,
        WIZ_PREV = 0x040c,
        CBEM_GETITEMW = 0x040d,
        RB_GETROWCOUNT = 0x040d,
        SB_GETTEXTW = 0x040d,
        TB_ISBUTTONINDETERMINATE = 0x040d,
        TTM_GETTOOLCOUNT = 0x040d,
        CBEM_SETEXTENDEDSTYLE = 0x040e,
        RB_GETROWHEIGHT = 0x040e,
        SB_ISSIMPLE = 0x040e,
        TB_ISBUTTONHIGHLIGHTED = 0x040e,
        TBM_GETPTICS = 0x040e,
        TTM_ENUMTOOLSA = 0x040e,
        SB_SETICON = 0x040f,
        TBM_GETTICPOS = 0x040f,
        TTM_GETCURRENTTOOLA = 0x040f,
        RB_IDTOINDEX = 0x0410,
        SB_SETTIPTEXTA = 0x0410,
        TBM_GETNUMTICS = 0x0410,
        TTM_WINDOWFROMPOINT = 0x0410,
        RB_GETTOOLTIPS = 0x0411,
        SB_SETTIPTEXTW = 0x0411,
        TBM_GETSELSTART = 0x0411,
        TB_SETSTATE = 0x0411,
        TTM_TRACKACTIVATE = 0x0411,
        RB_SETTOOLTIPS = 0x0412,
        SB_GETTIPTEXTA = 0x0412,
        TB_GETSTATE = 0x0412,
        TBM_GETSELEND = 0x0412,
        TTM_TRACKPOSITION = 0x0412,
        RB_SETBKCOLOR = 0x0413,
        SB_GETTIPTEXTW = 0x0413,
        TB_ADDBITMAP = 0x0413,
        TBM_CLEARSEL = 0x0413,
        TTM_SETTIPBKCOLOR = 0x0413,
        RB_GETBKCOLOR = 0x0414,
        SB_GETICON = 0x0414,
        TB_ADDBUTTONSA = 0x0414,
        TBM_SETTICFREQ = 0x0414,
        TTM_SETTIPTEXTCOLOR = 0x0414,
        RB_SETTEXTCOLOR = 0x0415,
        TB_INSERTBUTTONA = 0x0415,
        TBM_SETPAGESIZE = 0x0415,
        TTM_GETDELAYTIME = 0x0415,
        RB_GETTEXTCOLOR = 0x0416,
        TB_DELETEBUTTON = 0x0416,
        TBM_GETPAGESIZE = 0x0416,
        TTM_GETTIPBKCOLOR = 0x0416,
        RB_SIZETORECT = 0x0417,
        TB_GETBUTTON = 0x0417,
        TBM_SETLINESIZE = 0x0417,
        TTM_GETTIPTEXTCOLOR = 0x0417,
        RB_BEGINDRAG = 0x0418,
        TB_BUTTONCOUNT = 0x0418,
        TBM_GETLINESIZE = 0x0418,
        TTM_SETMAXTIPWIDTH = 0x0418,
        RB_ENDDRAG = 0x0419,
        TB_COMMANDTOINDEX = 0x0419,
        TBM_GETTHUMBRECT = 0x0419,
        TTM_GETMAXTIPWIDTH = 0x0419,
        RB_DRAGMOVE = 0x041a,
        TBM_GETCHANNELRECT = 0x041a,
        TB_SAVERESTOREA = 0x041a,
        TTM_SETMARGIN = 0x041a,
        RB_GETBARHEIGHT = 0x041b,
        TB_CUSTOMIZE = 0x041b,
        TBM_SETTHUMBLENGTH = 0x041b,
        TTM_GETMARGIN = 0x041b,
        RB_GETBANDINFOW = 0x041c,
        TB_ADDSTRINGA = 0x041c,
        TBM_GETTHUMBLENGTH = 0x041c,
        TTM_POP = 0x041c,
        RB_GETBANDINFOA = 0x041d,
        TB_GETITEMRECT = 0x041d,
        TBM_SETTOOLTIPS = 0x041d,
        TTM_UPDATE = 0x041d,
        RB_MINIMIZEBAND = 0x041e,
        TB_BUTTONSTRUCTSIZE = 0x041e,
        TBM_GETTOOLTIPS = 0x041e,
        TTM_GETBUBBLESIZE = 0x041e,
        RB_MAXIMIZEBAND = 0x041f,
        TBM_SETTIPSIDE = 0x041f,
        TB_SETBUTTONSIZE = 0x041f,
        TTM_ADJUSTRECT = 0x041f,
        TBM_SETBUDDY = 0x0420,
        TB_SETBITMAPSIZE = 0x0420,
        TTM_SETTITLEA = 0x0420,
        MSG_FTS_JUMP_VA = 0x0421,
        TB_AUTOSIZE = 0x0421,
        TBM_GETBUDDY = 0x0421,
        TTM_SETTITLEW = 0x0421,
        RB_GETBANDBORDERS = 0x0422,
        MSG_FTS_JUMP_QWORD = 0x0423,
        RB_SHOWBAND = 0x0423,
        TB_GETTOOLTIPS = 0x0423,
        MSG_REINDEX_REQUEST = 0x0424,
        TB_SETTOOLTIPS = 0x0424,
        MSG_FTS_WHERE_IS_IT = 0x0425,
        RB_SETPALETTE = 0x0425,
        TB_SETPARENT = 0x0425,
        RB_GETPALETTE = 0x0426,
        RB_MOVEBAND = 0x0427,
        TB_SETROWS = 0x0427,
        TB_GETROWS = 0x0428,
        TB_GETBITMAPFLAGS = 0x0429,
        TB_SETCMDID = 0x042a,
        RB_PUSHCHEVRON = 0x042b,
        TB_CHANGEBITMAP = 0x042b,
        TB_GETBITMAP = 0x042c,
        MSG_GET_DEFFONT = 0x042d,
        TB_GETBUTTONTEXTA = 0x042d,
        TB_REPLACEBITMAP = 0x042e,
        TB_SETINDENT = 0x042f,
        TB_SETIMAGELIST = 0x0430,
        TB_GETIMAGELIST = 0x0431,
        TB_LOADIMAGES = 0x0432,
        TTM_ADDTOOLW = 0x0432,
        TB_GETRECT = 0x0433,
        TTM_DELTOOLW = 0x0433,
        TB_SETHOTIMAGELIST = 0x0434,
        TTM_NEWTOOLRECTW = 0x0434,
        TB_GETHOTIMAGELIST = 0x0435,
        TTM_GETTOOLINFOW = 0x0435,
        TB_SETDISABLEDIMAGELIST = 0x0436,
        TTM_SETTOOLINFOW = 0x0436,
        TB_GETDISABLEDIMAGELIST = 0x0437,
        TTM_HITTESTW = 0x0437,
        TB_SETSTYLE = 0x0438,
        TTM_GETTEXTW = 0x0438,
        TB_GETSTYLE = 0x0439,
        TTM_UPDATETIPTEXTW = 0x0439,
        TB_GETBUTTONSIZE = 0x043a,
        TTM_ENUMTOOLSW = 0x043a,
        TB_SETBUTTONWIDTH = 0x043b,
        TTM_GETCURRENTTOOLW = 0x043b,
        TB_SETMAXTEXTROWS = 0x043c,
        TB_GETTEXTROWS = 0x043d,
        TB_GETOBJECT = 0x043e,
        TB_GETBUTTONINFOW = 0x043f,
        TB_SETBUTTONINFOW = 0x0440,
        TB_GETBUTTONINFOA = 0x0441,
        TB_SETBUTTONINFOA = 0x0442,
        TB_INSERTBUTTONW = 0x0443,
        TB_ADDBUTTONSW = 0x0444,
        TB_HITTEST = 0x0445,
        TB_SETDRAWTEXTFLAGS = 0x0446,
        TB_GETHOTITEM = 0x0447,
        TB_SETHOTITEM = 0x0448,
        TB_SETANCHORHIGHLIGHT = 0x0449,
        TB_GETANCHORHIGHLIGHT = 0x044a,
        TB_GETBUTTONTEXTW = 0x044b,
        TB_SAVERESTOREW = 0x044c,
        TB_ADDSTRINGW = 0x044d,
        TB_MAPACCELERATORA = 0x044e,
        TB_GETINSERTMARK = 0x044f,
        TB_SETINSERTMARK = 0x0450,
        TB_INSERTMARKHITTEST = 0x0451,
        TB_MOVEBUTTON = 0x0452,
        TB_GETMAXSIZE = 0x0453,
        TB_SETEXTENDEDSTYLE = 0x0454,
        TB_GETEXTENDEDSTYLE = 0x0455,
        TB_GETPADDING = 0x0456,
        TB_SETPADDING = 0x0457,
        TB_SETINSERTMARKCOLOR = 0x0458,
        TB_GETINSERTMARKCOLOR = 0x0459,
        TB_MAPACCELERATORW = 0x045a,
        TB_GETSTRINGW = 0x045b,
        TB_GETSTRINGA = 0x045c,
        TAPI_REPLY = 0x0463,
        ACM_OPENA = 0x0464,
        BFFM_SETSTATUSTEXTA = 0x0464,
        CDM_FIRST = 0x0464,
        CDM_GETSPEC = 0x0464,
        IPM_CLEARADDRESS = 0x0464,
        WM_CAP_UNICODE_START = 0x0464,
        ACM_PLAY = 0x0465,
        BFFM_ENABLEOK = 0x0465,
        CDM_GETFILEPATH = 0x0465,
        IPM_SETADDRESS = 0x0465,
        PSM_SETCURSEL = 0x0465,
        UDM_SETRANGE = 0x0465,
        WM_CHOOSEFONT_SETLOGFONT = 0x0465,
        ACM_STOP = 0x0466,
        BFFM_SETSELECTIONA = 0x0466,
        CDM_GETFOLDERPATH = 0x0466,
        IPM_GETADDRESS = 0x0466,
        PSM_REMOVEPAGE = 0x0466,
        UDM_GETRANGE = 0x0466,
        WM_CAP_SET_CALLBACK_ERRORW = 0x0466,
        WM_CHOOSEFONT_SETFLAGS = 0x0466,
        ACM_OPENW = 0x0467,
        BFFM_SETSELECTIONW = 0x0467,
        CDM_GETFOLDERIDLIST = 0x0467,
        IPM_SETRANGE = 0x0467,
        PSM_ADDPAGE = 0x0467,
        UDM_SETPOS = 0x0467,
        WM_CAP_SET_CALLBACK_STATUSW = 0x0467,
        BFFM_SETSTATUSTEXTW = 0x0468,
        CDM_SETCONTROLTEXT = 0x0468,
        IPM_SETFOCUS = 0x0468,
        PSM_CHANGED = 0x0468,
        UDM_GETPOS = 0x0468,
        CDM_HIDECONTROL = 0x0469,
        IPM_ISBLANK = 0x0469,
        PSM_RESTARTWINDOWS = 0x0469,
        UDM_SETBUDDY = 0x0469,
        CDM_SETDEFEXT = 0x046a,
        PSM_REBOOTSYSTEM = 0x046a,
        UDM_GETBUDDY = 0x046a,
        PSM_CANCELTOCLOSE = 0x046b,
        UDM_SETACCEL = 0x046b,
        EM_CONVPOSITION = 0x046c,
        PSM_QUERYSIBLINGS = 0x046c,
        UDM_GETACCEL = 0x046c,
        MCIWNDM_GETZOOM = 0x046d,
        PSM_UNCHANGED = 0x046d,
        UDM_SETBASE = 0x046d,
        PSM_APPLY = 0x046e,
        UDM_GETBASE = 0x046e,
        PSM_SETTITLEA = 0x046f,
        UDM_SETRANGE32 = 0x046f,
        PSM_SETWIZBUTTONS = 0x0470,
        UDM_GETRANGE32 = 0x0470,
        WM_CAP_DRIVER_GET_NAMEW = 0x0470,
        PSM_PRESSBUTTON = 0x0471,
        UDM_SETPOS32 = 0x0471,
        WM_CAP_DRIVER_GET_VERSIONW = 0x0471,
        PSM_SETCURSELID = 0x0472,
        UDM_GETPOS32 = 0x0472,
        PSM_SETFINISHTEXTA = 0x0473,
        PSM_GETTABCONTROL = 0x0474,
        PSM_ISDIALOGMESSAGE = 0x0475,
        MCIWNDM_REALIZE = 0x0476,
        PSM_GETCURRENTPAGEHWND = 0x0476,
        MCIWNDM_SETTIMEFORMATA = 0x0477,
        PSM_INSERTPAGE = 0x0477,
        MCIWNDM_GETTIMEFORMATA = 0x0478,
        PSM_SETTITLEW = 0x0478,
        WM_CAP_FILE_SET_CAPTURE_FILEW = 0x0478,
        MCIWNDM_VALIDATEMEDIA = 0x0479,
        PSM_SETFINISHTEXTW = 0x0479,
        WM_CAP_FILE_GET_CAPTURE_FILEW = 0x0479,
        MCIWNDM_PLAYTO = 0x047b,
        WM_CAP_FILE_SAVEASW = 0x047b,
        MCIWNDM_GETFILENAMEA = 0x047c,
        MCIWNDM_GETDEVICEA = 0x047d,
        PSM_SETHEADERTITLEA = 0x047d,
        WM_CAP_FILE_SAVEDIBW = 0x047d,
        MCIWNDM_GETPALETTE = 0x047e,
        PSM_SETHEADERTITLEW = 0x047e,
        MCIWNDM_SETPALETTE = 0x047f,
        PSM_SETHEADERSUBTITLEA = 0x047f,
        MCIWNDM_GETERRORA = 0x0480,
        PSM_SETHEADERSUBTITLEW = 0x0480,
        PSM_HWNDTOINDEX = 0x0481,
        PSM_INDEXTOHWND = 0x0482,
        MCIWNDM_SETINACTIVETIMER = 0x0483,
        PSM_PAGETOINDEX = 0x0483,
        PSM_INDEXTOPAGE = 0x0484,
        DL_BEGINDRAG = 0x0485,
        MCIWNDM_GETINACTIVETIMER = 0x0485,
        PSM_IDTOINDEX = 0x0485,
        DL_DRAGGING = 0x0486,
        PSM_INDEXTOID = 0x0486,
        DL_DROPPED = 0x0487,
        PSM_GETRESULT = 0x0487,
        DL_CANCELDRAG = 0x0488,
        PSM_RECALCPAGESIZES = 0x0488,
        MCIWNDM_GET_SOURCE = 0x048c,
        MCIWNDM_PUT_SOURCE = 0x048d,
        MCIWNDM_GET_DEST = 0x048e,
        MCIWNDM_PUT_DEST = 0x048f,
        MCIWNDM_CAN_PLAY = 0x0490,
        MCIWNDM_CAN_WINDOW = 0x0491,
        MCIWNDM_CAN_RECORD = 0x0492,
        MCIWNDM_CAN_SAVE = 0x0493,
        MCIWNDM_CAN_EJECT = 0x0494,
        MCIWNDM_CAN_CONFIG = 0x0495,
        IE_GETINK = 0x0496,
        IE_MSGFIRST = 0x0496,
        MCIWNDM_PALETTEKICK = 0x0496,
        IE_SETINK = 0x0497,
        IE_GETPENTIP = 0x0498,
        IE_SETPENTIP = 0x0499,
        IE_GETERASERTIP = 0x049a,
        IE_SETERASERTIP = 0x049b,
        IE_GETBKGND = 0x049c,
        IE_SETBKGND = 0x049d,
        IE_GETGRIDORIGIN = 0x049e,
        IE_SETGRIDORIGIN = 0x049f,
        IE_GETGRIDPEN = 0x04a0,
        IE_SETGRIDPEN = 0x04a1,
        IE_GETGRIDSIZE = 0x04a2,
        IE_SETGRIDSIZE = 0x04a3,
        IE_GETMODE = 0x04a4,
        IE_SETMODE = 0x04a5,
        IE_GETINKRECT = 0x04a6,
        WM_CAP_SET_MCI_DEVICEW = 0x04a6,
        WM_CAP_GET_MCI_DEVICEW = 0x04a7,
        WM_CAP_PAL_OPENW = 0x04b4,
        WM_CAP_PAL_SAVEW = 0x04b5,
        IE_GETAPPDATA = 0x04b8,
        IE_SETAPPDATA = 0x04b9,
        IE_GETDRAWOPTS = 0x04ba,
        IE_SETDRAWOPTS = 0x04bb,
        IE_GETFORMAT = 0x04bc,
        IE_SETFORMAT = 0x04bd,
        IE_GETINKINPUT = 0x04be,
        IE_SETINKINPUT = 0x04bf,
        IE_GETNOTIFY = 0x04c0,
        IE_SETNOTIFY = 0x04c1,
        IE_GETRECOG = 0x04c2,
        IE_SETRECOG = 0x04c3,
        IE_GETSECURITY = 0x04c4,
        IE_SETSECURITY = 0x04c5,
        IE_GETSEL = 0x04c6,
        IE_SETSEL = 0x04c7,
        CDM_LAST = 0x04c8,
        IE_DOCOMMAND = 0x04c8,
        MCIWNDM_NOTIFYMODE = 0x04c8,
        IE_GETCOMMAND = 0x04c9,
        IE_GETCOUNT = 0x04ca,
        IE_GETGESTURE = 0x04cb,
        MCIWNDM_NOTIFYMEDIA = 0x04cb,
        IE_GETMENU = 0x04cc,
        IE_GETPAINTDC = 0x04cd,
        MCIWNDM_NOTIFYERROR = 0x04cd,
        IE_GETPDEVENT = 0x04ce,
        IE_GETSELCOUNT = 0x04cf,
        IE_GETSELITEMS = 0x04d0,
        IE_GETSTYLE = 0x04d1,
        MCIWNDM_SETTIMEFORMATW = 0x04db,
        EM_OUTLINE = 0x04dc,
        MCIWNDM_GETTIMEFORMATW = 0x04dc,
        EM_GETSCROLLPOS = 0x04dd,
        EM_SETSCROLLPOS = 0x04de,
        EM_SETFONTSIZE = 0x04df,
        MCIWNDM_GETFILENAMEW = 0x04e0,
        MCIWNDM_GETDEVICEW = 0x04e1,
        MCIWNDM_GETERRORW = 0x04e4,
        FM_GETFOCUS = 0x0600,
        FM_GETDRIVEINFOA = 0x0601,
        FM_GETSELCOUNT = 0x0602,
        FM_GETSELCOUNTLFN = 0x0603,
        FM_GETFILESELA = 0x0604,
        FM_GETFILESELLFNA = 0x0605,
        FM_REFRESH_WINDOWS = 0x0606,
        FM_RELOAD_EXTENSIONS = 0x0607,
        FM_GETDRIVEINFOW = 0x0611,
        FM_GETFILESELW = 0x0614,
        FM_GETFILESELLFNW = 0x0615,
        WLX_WM_SAS = 0x0659,
        SM_GETSELCOUNT = 0x07e8,
        UM_GETSELCOUNT = 0x07e8,
        WM_CPL_LAUNCH = 0x07e8,
        SM_GETSERVERSELA = 0x07e9,
        UM_GETUSERSELA = 0x07e9,
        WM_CPL_LAUNCHED = 0x07e9,
        SM_GETSERVERSELW = 0x07ea,
        UM_GETUSERSELW = 0x07ea,
        SM_GETCURFOCUSA = 0x07eb,
        UM_GETGROUPSELA = 0x07eb,
        SM_GETCURFOCUSW = 0x07ec,
        UM_GETGROUPSELW = 0x07ec,
        SM_GETOPTIONS = 0x07ed,
        UM_GETCURFOCUSA = 0x07ed,
        UM_GETCURFOCUSW = 0x07ee,
        UM_GETOPTIONS = 0x07ef,
        UM_GETOPTIONS2 = 0x07f0,
        OCMBASE = 0x2000,
        OCM_CTLCOLOR = 0x2019,
        OCM_DRAWITEM = 0x202b,
        OCM_MEASUREITEM = 0x202c,
        OCM_DELETEITEM = 0x202d,
        OCM_VKEYTOITEM = 0x202e,
        OCM_CHARTOITEM = 0x202f,
        OCM_COMPAREITEM = 0x2039,
        OCM_NOTIFY = 0x204e,
        OCM_COMMAND = 0x2111,
        OCM_HSCROLL = 0x2114,
        OCM_VSCROLL = 0x2115,
        OCM_CTLCOLORMSGBOX = 0x2132,
        OCM_CTLCOLOREDIT = 0x2133,
        OCM_CTLCOLORLISTBOX = 0x2134,
        OCM_CTLCOLORBTN = 0x2135,
        OCM_CTLCOLORDLG = 0x2136,
        OCM_CTLCOLORSCROLLBAR = 0x2137,
        OCM_CTLCOLORSTATIC = 0x2138,
        OCM_PARENTNOTIFY = 0x2210,
        WM_APP = 0x8000,
        WM_RASDIALEVENT = 0xcccd
    }


    public enum WindowMessage
    {
        Null = 0,

        Create = 0x0001,
        Destroy = 0x0002,
        Move = 0x0003,
        Size = 0x0005,
        Activate = 0x0006,
        SetFocus = 0x0007,
        KillFocus = 0x0008,
        Enable = 0x000A,
        SetRedraw = 0x000B,
        SetText = 0x000C,
        GetText = 0x000D,
        GetTextLength = 0x000E,
        Paint = 0x000F,
        Close = 0x0010,
        QueryEndSession = 0x0011,
        Quit = 0x0012,
        QueryOpen = 0x0013,
        EraseBkgrnd = 0x0014,
        SysColorChange = 0x0015,
        EndSession = 0x0016,
        ShowWindow = 0x0018,
        CtlColor = 0x0019,
        WinInichange = 0x001A,
        DevModeChange = 0x001B,
        ActivateApp = 0x001C,
        FontChange = 0x001D,
        TimeChange = 0x001E,
        CancelMode = 0x001F,
        SetCursor = 0x0020,
        MouseActivate = 0x0021,
        ChildActivate = 0x0022,
        QueueSync = 0x0023,
        GetMinMaxInfo = 0x0024,

        PaintIcon = 0x0026,
        IconEraseBkgnd = 0x0027,
        NextDlgCtl = 0x0028,
        SpoolerStatus = 0x002A,

        DrawItem = 0x002b,
        MeasureItem = 0x002c,
        DeleteItem = 0x002d,
        VKeyToItem = 0x002e,
        CharToItem = 0x002f,
        SetFont = 0x0030,
        GetFont = 0x0031,
        SetHotKey = 0x0032,
        GetHotKey = 0x0033,

        QueryDragIcon = 0x0037,

        CompareItem = 0x0039,
        GetObject = 0x003d,

        Compacting = 0x0041,

        CommNotify = 0x0044,

        WindowPosChanging = 0x0046,
        WindowPosChanged = 0x0047,
        Power = 0x0048,
        CopyGlobalData = 0x0049,
        CopyData = 0x004a,
        CancelJournal = 0x004b,
        Notify = 0x004e,
        InputLangChangeRequest = 0x0050,
        InputLangChange = 0x0051,
        Card = 0x0052,
        Help = 0x0053,
        UserChanged = 0x0054,
        NotifyFormat = 0x0055,
        ContextMenu = 0x007b,

        StyleChanging = 0x007C,
        StyleChanged = 0x007D,
        DisplayChange = 0x007e,
        GetIcon = 0x007F,
        SetIcon = 0x0080,

        NcCreate = 0x0081,
        NcDestroy = 0x0082,
        NcCalcSize = 0x0083,
        NcHitTest = 0x0084,
        NcPaint = 0x0085,
        NcActivate = 0x0086,
        GetDlgCode = 0x0087,
        SyncPaint = 0x0088,
        NcMouseMove = 0x00a0,
        NcLButtonDown = 0x00a1,
        NcLButtonUp = 0x00a2,
        NcLButtonDblClk = 0x00a3,
        NcRButtonDown = 0x00a4,
        NcRButtonUp = 0x00a5,
        NcRButtonDblClk = 0x00a6,
        NcMButtonDown = 0x00a7,
        NcMButtonUp = 0x00a8,
        NcMButtonDblClk = 0x00a9,
        NcXButtonDown = 0x00ab,
        NcXButtonUp = 0x00ac,
        NcXButtonDblClk = 0x00ad,
        SbmSetPos = 0x00e0,
        SbmGetPos = 0x00e1,
        SbmSetRange = 0x00e2,
        SbmGetRange = 0x00e3,
        SbmEnableArrows = 0x00e4,
        SbmSetRangeRedraw = 0x00e6,
        SbmSetScrollInfo = 0x00e9,
        SbmGetScrollInfo = 0x00ea,
        SbmGetScrollBarInfo = 0x00eb,
        Input = 0x00ff,
        KeyDown = 0x0100,
        KeyFirst = 0x0100,
        KeyUp = 0x0101,
        Char = 0x0102,
        DeadChar = 0x0103,
        SysKeyDown = 0x0104,
        SysKeyUp = 0x0105,
        SysChar = 0x0106,
        SysDeadChar = 0x0107,
        Keylast = 0x0108,
        WntConvertRequestEx = 0x0109,
        ConvertRequest = 0x010a,
        ConvertResult = 0x010b,
        Interim = 0x010c,
        ImeStartcomposition = 0x010d,
        ImeEndcomposition = 0x010e,
        ImeComposition = 0x010f,
        ImeKeylast = 0x010f,
        Initdialog = 0x0110,
        Command = 0x0111,

        SysCommand = 0x0112,
        Timer = 0x0113,
        HScroll = 0x0114,
        VScroll = 0x0115,
        InitMenu = 0x0116,
        InitMenuPopup = 0x0117,
        SysTimer = 0x0118,
        MenuSelect = 0x011f,
        MenuChar = 0x0120,
        EnterIdle = 0x0121,
        MenuRButtonUp = 0x0122,
        MenuDrag = 0x0123,
        MenuGetObject = 0x0124,
        UnInitMenuPopup = 0x0125,
        MenuCommand = 0x0126,
        ChangeUiState = 0x0127,
        UpdateUiState = 0x0128,
        QueryUiState = 0x0129,
        CtlColorMsgBox = 0x0132,
        CtlColorEdit = 0x0133,
        CtlColorListBox = 0x0134,
        CtlColorBtn = 0x0135,
        CtlColorDlg = 0x0136,
        CtlColorScrollBar = 0x0137,
        CtlColorStatic = 0x0138,
        MouseFirst = 0x0200,
        MouseMove = 0x0200,
        LButtonDown = 0x0201,
        LButtonUp = 0x0202,
        LButtonDblClk = 0x0203,
        RButtonDown = 0x0204,
        RButtonUp = 0x0205,
        RButtonDblClk = 0x0206,
        MButtonDown = 0x0207,
        MButtonUp = 0x0208,
        MButtonDblClk = 0x0209,
        MouseLast = 0x0209,
        MouseWheel = 0x020a,
        XButtonDown = 0x020b,
        XButtonUp = 0x020c,
        XButtonDblClk = 0x020d,
        ParentNotify = 0x0210,
        EnterMenuLoop = 0x0211,
        ExitMenuLoop = 0x0212,
        NextMenu = 0x0213,

        Sizing = 0x0214,
        CaptureChanged = 0x0215,
        Moving = 0x0216,
        PowerBroadcast = 0x0218,
        DeviceChange = 0x0219,
        MdiCreate = 0x0220,
        MdiDestroy = 0x0221,
        MdiActivate = 0x0222,
        MdiRestore = 0x0223,
        MdiNext = 0x0224,
        MdiMaximize = 0x0225,
        MdiTile = 0x0226,
        MdiCascade = 0x0227,
        MdiIconArrange = 0x0228,
        MdiGetActive = 0x0229,
        MdiSetMenu = 0x0230,
        EnterSizeMode = 0x0231,
        ExitSizeMove = 0x0232,

        DropFiles = 0x0233,
        MdiRefreshMenu = 0x0234,
        ImeReport = 0x0280,
        ImeSetcontext = 0x0281,
        ImeNotify = 0x0282,
        ImeControl = 0x0283,
        ImeCompositionfull = 0x0284,
        ImeSelect = 0x0285,
        ImeChar = 0x0286,
        ImeRequest = 0x0288,
        ImeKeyDown = 0x0290,
        ImeKeyUp = 0x0291,
        NcMouseHover = 0x02a0,
        MouseHover = 0x02a1,
        NcMouseLeave = 0x02a2,
        MouseLeave = 0x02a3,
        Cut = 0x0300,
        Copy = 0x0301,
        Paste = 0x0302,
        Clear = 0x0303,
        Undo = 0x0304,
        RenderFormat = 0x0305,
        RenderAllFormats = 0x0306,
        DestroyClipboard = 0x0307,
        DrawClipboard = 0x0308,
        PaintClipboard = 0x0309,
        VScrollClipboard = 0x030a,
        SizeClipboard = 0x030b,
        AskCbFormatName = 0x030c,
        ChangeCbChain = 0x030d,
        HScrollClipboard = 0x030e,
        QueryNewPalette = 0x030f,
        PaletteIsChanging = 0x0310,
        PaletteChanged = 0x0311,
        HotKey = 0x0312,
        Print = 0x0317,
        PrintClient = 0x0318,
        AppCommand = 0x0319,
        ThemeChanged = 0x031A,
        HandHeldFirst = 0x0358,
        HandHeldLast = 0x035f,
        AfxFirst = 0x0360,
        AfxLast = 0x037f,
        PenWinFirst = 0x0380,
        RcResult = 0x0381,
        HookRcResult = 0x0382,
        GlobalRcChange = 0x0383,
        PenMiscInfo = 0x0383,
        Skb = 0x0384,
        HEditCtl = 0x0385,
        PenCtl = 0x0385,
        PenMisc = 0x0386,
        CtlInit = 0x0387,
        PenEvent = 0x0388,
        PenWinLast = 0x038f,

        User = 0x0400,

        WmPsdPagesetupdlg = 0x0400,
        WmPsdFullpagerect = 0x0401,
        WmPsdMinmarginrect = 0x0402,
        WmPsdGreektextrect = 0x0404,
        WmPsdEnvstamprect = 0x0405,
        WmPsdYafullpagerect = 0x0406,

        WmChoosefontGetlogfont = 0x0401,
        WmChoosefontSetflags = 0x0466,

        WmCapUnicodeStart = 0x0464,
        WmCapSetCallbackErrorw = 0x0466,
        WmCapSetCallbackStatusw = 0x0467,
        WmCapDriverGetNamew = 0x0470,
        WmCapDriverGetVersionw = 0x0471,
        WmCapFileSetCaptureFilew = 0x0478,
        WmCapFileGetCaptureFilew = 0x0479,
        WmCapFileSaveasw = 0x047b,
        WmCapFileSavedibw = 0x047d,


        WmCapSetMciDevicew = 0x04a6,
        WmCapGetMciDevicew = 0x04a7,
        WmCapPalOpenw = 0x04b4,
        WmCapPalSavew = 0x04b5,


        WmCplLaunched = 0x07e9,

        WmApp = 0x8000,
        WmRasDialEvent = 0xcccd

    }
    public enum IconSize : int
    {
        Small = 0x0,
        Big = 0x1,
        Small2 = 0x2,
    }

    public enum GetClassWindow
    {
        /// <summary>
        /// Retrieves an ATOM value that uniquely identifies the window class. This is the same atom that the RegisterClassEx function returns. 
        /// </summary>
        Atom = -32,
        /// <summary>
        /// Retrieves the size, in bytes, of the extra memory associated with the class. 
        /// </summary>
        CbClsExtra = -20,
        /// <summary>
        /// Retrieves the size, in bytes, of the extra window memory associated with each window in the class. For information on how to access this memory, see GetWindowLongPtr. 
        /// </summary>
        CbWndExtra = -18,
        /// <summary>
        /// Retrieves a handle to the background brush associated with the class. 
        /// </summary>
        HBrBackground = -10,
        /// <summary>
        /// Retrieves a handle to the cursor associated with the class. 
        /// </summary>
        HCursor = -12,
        /// <summary>
        /// Retrieves a handle to the icon associated with the class. 
        /// </summary>
        HIcon = -14,
        /// <summary>
        /// Retrieves a handle to the small icon associated with the class. 
        /// </summary>
        HIconSm = -34,
        /// <summary>
        /// Retrieves a handle to the module that registered the class. 
        /// </summary>
        HModule = -16,
        /// <summary>
        /// Retrieves the pointer to the menu name string. The string identifies the menu resource associated with the class. 
        /// </summary>
        MenuName = -8,
        /// <summary>
        /// Retrieves the window-class style bits. 
        /// </summary>
        Style = -26,
        /// <summary>
        /// Retrieves the address of the window procedure, or a handle representing the address of the window procedure. You must use the CallWindowProc function to call the window procedure. 
        /// </summary>
        WndProc = -24,
    }

    //2297
    [LibraryImport("user32.dll")]
    public static partial int SendMessage(nint hWnd, WindowMessage wMsg, [MarshalAs(UnmanagedType.Bool)] bool wParam, int lParam);

    [LibraryImport("user32.dll")]
    public static partial int SendMessage(nint hWnd, WindowMessage wMsg, nint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    public static partial int SendMessage(nint hWnd, WindowMessage wMsg, int wParam, int lParam);


    public enum PredefinedIcons
    {
        /// <summary>
        /// Default application icon. 
        /// </summary>
        Application = 0x7F00,

        /// <summary>
        /// Hand-shaped icon. 
        /// </summary>
        Error = 0x7F01,
        /// <summary>
        /// Hand-shaped icon. Same as IDI_ERROR. 
        /// </summary>
        Hand = 0x7F01,

        /// <summary>
        /// Question mark icon. 
        /// </summary>
        Question = 0x7F02,

        /// <summary>
        /// Exclamation point icon. 
        /// </summary>
        Warning = 0x7F03,
        /// <summary>
        /// Exclamation point icon. Same as IDI_WARNING. 
        /// </summary>
        Exclamation = 0x7F03,

        /// <summary>
        /// Asterisk icon. 
        /// </summary>
        Information = 0x7F04,
        /// <summary>
        /// Asterisk icon. Same as IDI_INFORMATION. 
        /// </summary>
        Asterisk = 0x7F04,

        /// <summary>
        /// Default application icon.
        /// Windows 2000:  Windows logo icon.
        /// </summary>
        WinLogo = 0x7F05,

        /// <summary>
        /// Security Shield icon. 
        /// </summary>
        Shield = 0x7F06,
    }


    //[LibraryImport("user32.dll")]
    //internal static partial nint LoadIcon(nint hInstance, string lpIconName);

    [LibraryImport("user32.dll")]
    internal static partial nint LoadIcon(nint hInstance, PredefinedIcons lpIconName);



    [DllImport("user32.dll", EntryPoint = "GetClassLong")]
    internal static extern uint GetClassLong32(nint hWnd, GetClassWindow nIndex);

    [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
    internal static extern Int64 GetClassLong64(nint hWnd, GetClassWindow nIndex);

    internal static nint GetClassLongPtr(nint hWnd, GetClassWindow nIndex)
    {
        if (Environment.Is64BitProcess) return (nint)GetClassLong64(hWnd, nIndex);

        return (nint)GetClassLong32(hWnd, nIndex);
    }


    public enum SystemParametersInfoAction : uint
    {
        SetCursors = 0x57,

        GetMouseSpeed = 0x70,
        SetMouseSpeed = 0x71,

        GetDeskWallpaper = 0x73,
        SetDeskWallpaper = 0x14,
    }

    [Flags]
    public enum SystemParametersInfoFlags : uint
    {
        UpdateIniFile = 0x01,
        SendWinIniChange = 0x02,
    }

    [DllImport("User32.dll")]
    public static extern bool SystemParametersInfo(
        SystemParametersInfoAction uiAction,
        uint uiParam,
        ref uint pvParam,
        SystemParametersInfoFlags fWinIni);


    [DllImport("User32.dll")]
    public static extern bool SystemParametersInfo(
        SystemParametersInfoAction uiAction,
        uint uiParam,
        uint pvParam,
        SystemParametersInfoFlags fWinIni);

    //2432
    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int SystemParametersInfo(SystemParametersInfoAction uAction, int uParam, string lpvParam, int fuWinIni);

    [LibraryImport("user32.dll")]
    public static partial int GetWindowLong(nint hwnd, int index);


    public enum WindowLongFlags : int
    {
        ExStyle = -20,
        P_HInstance = -6,
        P_HwndParent = -8,
        Id = -12,
        Style = -16,
        UserData = -21,
        WndProc = -4,
        PUser = 0x8,
        PMsgResult = 0x0,
        PDlgProc = 0x4
    }


    [LibraryImport("user32.dll")]
    public static partial int SetWindowLong(IntPtr hwnd, WindowLongFlags index, int newStyle);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetCursorPos(int x, int y);

    [LibraryImport("User32.dll")]
    public static partial int PhysicalToLogicalPoint( //ForPerMonitorDPI(
        nint hwnd,
        ref Point lpPoint
    );

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);


    #endregion

    #region Hight DPI




    // https://learn.microsoft.com/en-us/windows/win32/api/_hidpi/

    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint GetThreadDpiAwarenessContext();

    [DllImport("SHcore.dll")]
    public static extern int GetProcessDpiAwareness(nint hWnd, out DpiAwareness value);

    [DllImport("user32.dll")]
    public static extern int GetDpiForWindow(nint hWnd);

    [DllImport("user32.dll")]
    public static extern nint GetWindowDpiAwarenessContext(nint hWnd);

    [DllImport("user32.dll")]
    public static extern DpiAwareness GetAwarenessFromDpiAwarenessContext(nint dpiContext);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AreDpiAwarenessContextsEqual(nint dpiContextA, nint dpiContextB);









    [LibraryImport("User32.dll")]
    public static partial int LogicalToPhysicalPointForPerMonitorDPI(
        nint hwnd,
        ref Point lpPoint
    );

    #endregion

    #region Windows and Messages

    //1852
    [LibraryImport("user32.dll")]
    public static partial nint GetForegroundWindow();

    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(int nIndex);

    [LibraryImport("user32.dll")]
    public static partial int SendMessage(nint hWnd, int Msg, int wParam, int lParam);

    [LibraryImport("user32.dll")]
    public static partial int SendMessage(int hWnd, int Msg, int wParam, int lParam);

    #endregion

    #region Keyboard and Mouse Input

    [LibraryImport("user32.dll")]
    public static partial int MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    public static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, nint dwhkl);


    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SendInput(int nInputs, ref WinUser.Input mi, int cbSize);


    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial short VkKeyScan(char ch);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern short VkKeyScanEx(char ch, nint dwhkl);

    #endregion

}