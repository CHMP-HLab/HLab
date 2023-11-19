using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API;

public static partial class ShlObjCore
{


    [DllImport("Shell32.dll", SetLastError = false)]
    public static extern int SHDefExtractIcon(string iconFile, int iconIndex, uint flags, ref nint hiconLarge, ref nint hiconSmall, uint iconSize);

}