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
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace HLab.Sys.Windows.API;

//[System.Security.SuppressUnmanagedCodeSecurity]
public static partial class Gdi32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct RAMP
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Red;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Green;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Blue;
    }



    [DllImport("gdi32.dll")]
    public static extern int GetDeviceGammaRamp(nint hDC, ref RAMP lpRamp);

    [DllImport("gdi32.dll")]
    public static extern int SetDeviceGammaRamp(nint hDC, ref RAMP lpRamp);

    [DllImport("gdi32.dll", EntryPoint = "DDCCIGetCapabilitiesStringLength", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool DDCCIGetCapabilitiesStringLength(
        [In] nint hMonitor, out uint pdwLength);

    [DllImport("gdi32.dll", EntryPoint = "DDCCIGetCapabilitiesString", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool DDCCIGetCapabilitiesString(
        [In] nint hMonitor, StringBuilder pszString, uint dwLength);

    public static string DDCCIGetCapabilitiesString(nint hMonitor)
    {
        if (DDCCIGetCapabilitiesStringLength(hMonitor, out var len))
        {
            if (len == 0) return null;

            var s = new StringBuilder((int)len + 1);
            if (DDCCIGetCapabilitiesString(hMonitor, s, len))
            {
                return s.ToString();
            }
        }
        return null;
    }
}