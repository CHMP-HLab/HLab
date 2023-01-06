/*
  HLab.Windows.API
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Windows.API.

    HLab.Windows.API is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Windows.API is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace HLab.Sys.Windows.API
{
    public static partial class User32
    {


        [Flags()]
        public enum DM : int
        {
            Orientation = 0x1,
            PaperSize = 0x2,
            PaperLength = 0x4,
            PaperWidth = 0x8,
            Scale = 0x10,
            Position = 0x20,
            NUP = 0x40,
            DisplayOrientation = 0x80,
            Copies = 0x100,
            DefaultSource = 0x200,
            PrintQuality = 0x400,
            Color = 0x800,
            Duplex = 0x1000,
            YResolution = 0x2000,
            TTOption = 0x4000,
            Collate = 0x8000,
            FormName = 0x10000,
            LogPixels = 0x20000,
            BitsPerPixel = 0x40000,
            PelsWidth = 0x80000,
            PelsHeight = 0x100000,
            DisplayFlags = 0x200000,
            DisplayFrequency = 0x400000,
            ICMMethod = 0x800000,
            ICMIntent = 0x1000000,
            MediaType = 0x2000000,
            DitherType = 0x4000000,
            PanningWidth = 0x8000000,
            PanningHeight = 0x10000000,
            DisplayFixedOutput = 0x20000000
        }












        internal enum DEVICE_SCALE_FACTOR : short
        {
            SCALE_100_PERCENT = 100,
            SCALE_120_PERCENT = 120,
            SCALE_140_PERCENT = 140,
            SCALE_150_PERCENT = 150,
            SCALE_160_PERCENT = 160,
            SCALE_180_PERCENT = 180,
            SCALE_225_PERCENT = 225,
        }


        public enum Process_DPI_Awareness
        {
            Unaware = 0,
            System_DPI_Aware = 1,
            Per_Monitor_DPI_Aware = 2
        }

        public enum DPI_Awareness_Context
        {
            Unset = 0,
            Unaware = 16,
            System_Aware = 17,
            Per_Monitor_Aware = 18,
            Per_Monitor_Aware_V2 = 34,
        }






        //Input

        public const uint SPI_SETCURSORS = 0x0057;
        public const uint SPIF_UPDATEINIFILE = 0x01;
        public const uint SPIF_SENDCHANGE = 0x02;
        public const uint SPI_SETMOUSESPEED = 0x0071;
        public const uint SPI_GETMOUSESPEED = 0x0070;


        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [LibraryImport("user32.dll")]
        public static partial int GetWindowLong(IntPtr hwnd, int index);

        [LibraryImport("user32.dll")]
        public static partial int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            uint pvParam,
            uint fWinIni);

        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            ref uint pvParam,
            uint fWinIni);


        [LibraryImport("User32.dll")]
        public static partial int PhysicalToLogicalPoint( //ForPerMonitorDPI(
            IntPtr hwnd,
            ref WinDef.Point lpPoint
            );

        // LogicalToPhysicalPointForPerMonitorDPI

        [LibraryImport("User32.dll")]
        public static partial int LogicalToPhysicalPointForPerMonitorDPI(
            IntPtr hwnd,
            ref WinDef.Point lpPoint
            );
        public const int WM_SETREDRAW = 11;

        public const int SC_MONITORPOWER = 0xF170;
        public const int WM_SYSCOMMAND = 0x0112;

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        public enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        public enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }
        public const uint SPI_GETDESKWALLPAPER = 0x73;

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
        public static extern bool ClipCursor(ref WinDef.Rect lpRect);

        //1852
        [LibraryImport("user32.dll")]
        public static partial nint GetForegroundWindow();

        //1961
        [LibraryImport("user32.dll")]
        internal static partial int GetSystemMetrics(int nIndex);

        //1967
        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial DPI_Awareness_Context GetThreadDpiAwarenessContext();

        //2141-2144
        [LibraryImport("user32.dll")]
        public static partial int MapVirtualKey(int nVirtKey, int nMapType);

        //2291
        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial int SendInput(int nInputs, ref WinUser.Input mi, int cbSize);


        //2387
        [LibraryImport("user32.dll")]
        public static partial int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        //2432
        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int SystemParametersInfo(uint uAction, int uParam, string lpvParam, int fuWinIni);

        //2486 
        [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial short VkKeyScan(char ch);

        [LibraryImport("user32.dll")]
        public static partial int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [LibraryImport("user32.dll")]
        public static partial int SendMessage(int hWnd, int Msg, int wParam, int lParam);
    }
}
