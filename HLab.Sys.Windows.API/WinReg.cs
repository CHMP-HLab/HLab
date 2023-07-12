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
using Microsoft.Win32;

// ReSharper disable InconsistentNaming

namespace HLab.Sys.Windows.API;

[global::System.Security.SuppressUnmanagedCodeSecurity]
public static partial class WinReg
{
    public static RegistryKey RegistryKey(nint hKey, int parent = 0) => RegistryKey(GetHKeyName(hKey), parent);
    public static RegistryKey RegistryKey(string path, int parent = 0)
    {
        var keys = path.Split('\\');

        if (keys.Length < 3) throw new InvalidOperationException("path is not valid.");

        var key = keys[2] switch
        {
            "USER" => Registry.CurrentUser,
            "CONFIG" => Registry.CurrentConfig,
            _ => Registry.LocalMachine
        };

        for (var i = 3; i < (keys.Length - parent); i++)
        {
            if (key == null) return key;
            key = key.OpenSubKey(keys[i]);
        }

        return key;
    }

    public static string GetHKeyName(nint hKey)
    {
        var result = string.Empty;

        var status = Wdm.ZwQueryKey(hKey, Wdm.KeyInformationClass.KeyNameInformation, 0, 0, out var needed);
        if (status != 0xC0000023) return result;

        var pKni = Marshal.AllocHGlobal(cb: sizeof(uint) + needed + 4 /*paranoia*/);
        status = Wdm.ZwQueryKey(hKey, Wdm.KeyInformationClass.KeyNameInformation, pKni, needed, out needed);
        if (status == 0)    // STATUS_SUCCESS
        {
            var bytes = new char[2 + needed + 2];
            Marshal.Copy(pKni, bytes, 0, needed);
            // startIndex == 2  skips the NameLength field of the structure (2 chars == 4 bytes)
            // needed/2         reduces value from bytes to chars
            //  needed/2 - 2    reduces length to not include the NameLength
            result = new string(bytes, 2, (needed / 2) - 2);
        }
        Marshal.FreeHGlobal(pKni);
        return result;
    }


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
    public static partial int RegCloseKey(nint hKey);
}