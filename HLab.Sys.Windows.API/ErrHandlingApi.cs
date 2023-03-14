using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API;

public static partial class ErrHandlingApi
{
    public static int GetLastError() => Marshal.GetLastWin32Error();

    public static string GetLastErrorString()
    {
        var lastError = GetLastError();
        if (lastError == 0) return "";

        var size = WinBase.FormatMessage(WinBase.FormatMessageFlags.AllocateBuffer | WinBase.FormatMessageFlags.FromSystem | WinBase.FormatMessageFlags.IgnoreInserts,
            0, lastError, 0, out var msgOut, 256, 0);
        return msgOut.ToString().Trim();
    }
}