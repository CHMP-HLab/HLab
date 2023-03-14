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

// ReSharper disable InconsistentNaming

namespace HLab.Sys.Windows.API
{
    [global::System.Security.SuppressUnmanagedCodeSecurity]
    public static partial class WinReg
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint RegEnumValue(
              nint hKey,
              uint dwIndex,
              StringBuilder lpValueName,
              ref uint lpcValueName,
              nint lpReserved,
              ref uint lpType,
              nint lpData,
              ref uint lpcbData);

        [LibraryImport("advapi32.dll", SetLastError = true)]
        public static partial int RegCloseKey(
            nint hKey);
    }
}
