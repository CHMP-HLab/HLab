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
using System.Text;

namespace HLab.Sys.Windows.API;

//[System.Security.SuppressUnmanagedCodeSecurity]
public static partial class WinBase
{
    public const uint ErrorNoMoreItems = 259;

    [Flags]
    public enum FormatMessageFlags : uint
    {
        AllocateBuffer = 0x00000100,
        IgnoreInserts = 0x00000200,
        FromSystem = 0x00001000,
        ArgumentArray = 0x00002000,
        FromHModule = 0x00000800,
        FromString = 0x00000400
    }
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern int FormatMessage(FormatMessageFlags dwFlags, nint lpSource, int dwMessageId, uint dwLanguageId, out StringBuilder msgOut, int nSize, nint Arguments);

}