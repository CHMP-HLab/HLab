/*
  LittleBigMouse.Screen.Config
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of LittleBigMouse.Screen.Config.

    LittleBigMouse.Screen.Config is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    LittleBigMouse.Screen.Config is distributed in the hope that it will be useful,
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
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace HLab.Base.Wpf
{
    public static class WindowExtensions
    {
        public static void EnableBlur(this Window window, bool enable = true)
        {
            // TODO : fix moving 
            if(
                Environment.OSVersion.Platform == PlatformID.Win32NT 
                && Environment.OSVersion.Version.Major == 10 
                && Environment.OSVersion.Version.Build is < 22000 or >= 22563
                )
            {
                    new BlurHelper(window, enable);
            }
        }
    }

    internal class BlurHelper
    {
        readonly WeakReference<Window> _window;
        readonly bool _enable;

        public BlurHelper(Window window, bool enable)
        {
                _enable= enable;
                _window = new WeakReference<Window>(window);

                if(window.IsLoaded)
                    Window_Loaded(null,null);
                else
                    window.Loaded += Window_Loaded;

        }


        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_window == null || !_window.TryGetTarget(out var window)) return;

            window.Loaded -= Window_Loaded;

            var windowHelper = new WindowInteropHelper(window);

            var accent = new NativeMethods.AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = _enable?NativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND:NativeMethods.AccentState.ACCENT_DISABLED; //NativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new NativeMethods.WindowCompositionAttributeData
            {
                Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            _ = NativeMethods.SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
            _window.SetTarget(null);
        }
    }

    internal static partial class NativeMethods
    {
        public enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_5 = 5,
            ACCENT_6 = 6,
            ACCENT_7 = 7,
            ACCENT_8 = 8,
            ACCENT_9 = 9
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
            WCA_ACCENT_POLICY = 19
            // ...
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    }
}
