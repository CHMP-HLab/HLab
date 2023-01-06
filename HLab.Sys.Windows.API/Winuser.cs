using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static HLab.Sys.Windows.API.WinDef;

namespace HLab.Sys.Windows.API;

public static partial class WinUser
{
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

    [NativeMarshalling(typeof(DevModeMarshaller))]
    public struct DevMode
    {
        public DevMode() { }

        public string DeviceName;
        public short SpecVersion;
        public short DriverVersion;
        public short DriverExtra;
        public User32.DM Fields;
        public short Orientation;
        public short PaperSize;
        public short PaperLength;
        public short PaperWidth;
        public short Scale;
        public short Copies;
        public short DefaultSource;
        public short PrintQuality;
        public WinDef.PointL Position;
        public int DisplayOrientation;
        public int DisplayFixedOutput;
        public short Color; // See note below!
        public short Duplex; // See note below!
        public short YResolution;
        public short TTOption;
        public short Collate; // See note below!
        public string FormName;
        public short LogPixels;
        public int BitsPerPel;
        public int PelsWidth;
        public int PelsHeight;
        public int DisplayFlags;
        public int Nup;
        public int DisplayFrequency;
    }

    [CustomMarshaller(typeof(DevMode), MarshalMode.ManagedToUnmanagedRef, typeof(DevModeMarshaller))]
    internal static unsafe class DevModeMarshaller
    {
        // Unmanaged representation of ErrorData.
        // Should mimic the unmanaged error_data type at a binary level.
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        internal struct DevModeUnmanaged
        {
            [FieldOffset(0)]
            public fixed char DeviceName[32];
            [FieldOffset(32)]
            public short SpecVersion;
            [FieldOffset(34)]
            public short DriverVersion;
            [FieldOffset(36)]
            public short Size;
            [FieldOffset(38)]
            public short DriverExtra;
            [FieldOffset(40)]
            public User32.DM Fields;
            [FieldOffset(44)]
            public short Orientation;
            [FieldOffset(46)]
            public short PaperSize;
            [FieldOffset(48)]
            public short PaperLength;
            [FieldOffset(50)]
            public short PaperWidth;
            [FieldOffset(52)]
            public short Scale;
            [FieldOffset(54)]
            public short Copies;
            [FieldOffset(56)]
            public short DefaultSource;
            [FieldOffset(58)]
            public short PrintQuality;
            [FieldOffset(44)]
            public WinDef.PointL Position;
            [FieldOffset(52)]
            public int DisplayOrientation;
            [FieldOffset(56)]
            public int DisplayFixedOutput;

            [FieldOffset(60)]
            public short Color; // See note below!
            [FieldOffset(62)]
            public short Duplex; // See note below!
            [FieldOffset(64)]
            public short YResolution;
            [FieldOffset(66)]
            public short TTOption;
            [FieldOffset(68)]
            public short Collate; // See note below!
            [FieldOffset(72)]
            public fixed char FormName[32];
            [FieldOffset(102)]
            public short LogPixels;
            [FieldOffset(104)]
            public int BitsPerPel;
            [FieldOffset(108)]
            public int PelsWidth;
            [FieldOffset(112)]
            public int PelsHeight;
            [FieldOffset(116)]
            public int DisplayFlags;
            [FieldOffset(116)]
            public int Nup;
            [FieldOffset(120)]
            public int DisplayFrequency;
        }

        public static DevModeUnmanaged ConvertToUnmanaged(DevMode managed)
        {
            var devMode = new DevModeUnmanaged ()
            {
                SpecVersion = managed.SpecVersion,
                DriverVersion = managed.DriverVersion,
                Size = (short)sizeof(DevModeUnmanaged),
                DriverExtra = managed.DriverExtra,
                Fields = managed.Fields,
                Orientation = managed.Orientation,
                PaperSize = managed.PaperSize,
                PaperLength = managed.PaperLength,
                PaperWidth = managed.PaperWidth,
                Scale = managed.Scale,
                Copies = managed.Copies,
                DefaultSource = managed.DefaultSource,
                PrintQuality = managed.PrintQuality,
                Position = managed.Position,
                DisplayOrientation = managed.DisplayOrientation,
                DisplayFixedOutput = managed.DisplayFixedOutput,
                Color = managed.Color,
                Duplex = managed.Duplex,
                YResolution = managed.YResolution,
                TTOption = managed.TTOption,
                Collate = managed.Collate,
                LogPixels = managed.LogPixels,
                BitsPerPel = managed.BitsPerPel,
                PelsWidth = managed.PelsWidth,
                PelsHeight = managed.PelsHeight,
                DisplayFlags = managed.DisplayFlags,
                Nup = managed.Nup,
                DisplayFrequency = managed.DisplayFrequency,
            };

            for (int i = 0; i < 32; i++)
            {
                devMode.DeviceName[i] = managed.DeviceName[i];
                devMode.FormName[i] = managed.FormName[i];
            }

            //var deviceName = Marshal.PtrToStringAnsi(new IntPtr(devMode.DeviceName), 32).ToCharArray();
            //Array.Copy(deviceName, devMode.DeviceName, 32);

            return devMode;
        }

        public static DevMode ConvertToManaged(DevModeUnmanaged umanaged)
        {
            var devMode = new DevMode ()
            {
                DeviceName = Marshal.PtrToStringAnsi(new IntPtr(umanaged.DeviceName), 32),
                SpecVersion = umanaged.SpecVersion,
                DriverVersion = umanaged.DriverVersion,
                DriverExtra = umanaged.DriverExtra,
                Fields = umanaged.Fields,
                Orientation = umanaged.Orientation,
                PaperSize = umanaged.PaperSize,
                PaperLength = umanaged.PaperLength,
                PaperWidth = umanaged.PaperWidth,
                Scale = umanaged.Scale,
                Copies = umanaged.Copies,
                DefaultSource = umanaged.DefaultSource,
                PrintQuality = umanaged.PrintQuality,
                Position = umanaged.Position,
                DisplayOrientation = umanaged.DisplayOrientation,
                DisplayFixedOutput = umanaged.DisplayFixedOutput,
                Color = umanaged.Color,
                Duplex = umanaged.Duplex,
                YResolution = umanaged.YResolution,
                TTOption = umanaged.TTOption,
                Collate = umanaged.Collate,
                LogPixels = umanaged.LogPixels,
                BitsPerPel = umanaged.BitsPerPel,
                PelsWidth = umanaged.PelsWidth,
                PelsHeight = umanaged.PelsHeight,
                DisplayFlags = umanaged.DisplayFlags,
                Nup = umanaged.Nup,
                DisplayFrequency = umanaged.DisplayFrequency,
            };
            return devMode;
        }

        public static void Free(DevModeUnmanaged unmanaged)
            => throw new NotImplementedException();
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

    //Display
    [LibraryImport("user32.dll")]
    public static partial DispChange ChangeDisplaySettings(ref DevMode devMode, ChangeDisplaySettingsFlags flags);

    [LibraryImport("user32.dll")]
    public static partial DispChange ChangeDisplaySettings(nint devMode, ChangeDisplaySettingsFlags flags);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial DispChange ChangeDisplaySettingsEx(string lpszDeviceName, ref DevMode lpDevMode, nint hwnd, ChangeDisplaySettingsFlags dwflags, nint lParam);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial DispChange ChangeDisplaySettingsEx(string lpszDeviceName, nint lpDevMode, nint hwnd, ChangeDisplaySettingsFlags dwflags, nint lParam);


    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumDisplaySettings(string deviceName, int modeNum, ref DevMode devMode);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumDisplaySettingsEx(string deviceName, int modeNum, ref DevMode devMode, int dwFlags);

    #region GetMonitorInfo

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        public int Size = (int)Marshal.SizeOf(typeof(MonitorInfoEx));
        public WinDef.Rect Monitor = default;
        public WinDef.Rect WorkArea = default;
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
    public unsafe struct MonitorInfoEx
    {
        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function. 
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int Size = (int)Marshal.SizeOf(typeof(MonitorInfoEx));

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. 
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public WinDef.Rect Monitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications, 
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor. 
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars. 
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public WinDef.Rect WorkArea;

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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public fixed char DeviceName[32];

        public MonitorInfoEx() { }
    }

    //Monitor
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMonitorInfo(nint hMonitor, ref MonitorInfo lpmi);

    #endregion


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
        public UIntPtr dwExtraInfo;
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
        => SetWindowPos(hWnd,(nint)hWndInsertAfter,x,y,cx,cy,uFlags);

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

    public delegate bool EnumWindowsProc(nint hWnd, IntPtr lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumDesktopWindows(nint hDesktop, EnumWindowsProc ewp, int lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out WinDef.Point lpPoint);


    public enum WindowMessage
    {
        Null = 0,

        Create = 0x0001,
        Destroy = 0x0002,
        Move = 0x0003,
        Size = 0x0005,
        Enable = 0x000A,
        Close = 0x0010,
        Quit = 0x0012,
        QueryOpen = 0x0013,

        ShowWindow = 0x0018,
        ActivateApp = 0x001C,
        CancelMode = 0x001F,
        ChildActivate = 0x0022,
        GetMinMaxInfo = 0x0024,
        QueryDragIcon = 0x0037,
        Compacting = 0x0041,
        WindowPosChanging = 0x0046,
        WindowPosChanged = 0x0047,
        InputLangChangeRequest = 0x0050,
        InputLangChange = 0x0051,
        UserChanged = 0x0054,

        StyleChanging = 0x007C,
        StyleChanged = 0x007D,
        GetIcon = 0x007F,

        NcCreate = 0x0081,
        NcDestroy = 0x0082,
        NcCalcSize = 0x0083,
        NcActivate = 0x0086,

        Sizing = 0x0214,
        Moving = 0x0216,
        EnterSizeMode = 0x0231,
        ExitSizeMove = 0x0232,

        ThemeChanged = 0x031A,
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


    [LibraryImport("user32.dll")]
    internal static partial nint LoadIcon(nint hInstance, string lpIconName);

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
}