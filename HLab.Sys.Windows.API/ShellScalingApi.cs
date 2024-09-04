using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API;

public static partial class ShellScalingApi
{
    #region High DPI
    public enum ProcessDpiAwareness
    {
        Unaware = 0,
        SystemDpiAware = 1,
        PerMonitorDpiAware = 2
    }

    public enum DpiType : int
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
        Default = Effective
    }        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx


    //[LibraryImport("Shcore.dll")]
    //public static partial nint GetDpiForMonitor(in nint hmonitor, in DpiType dpiType, out uint dpiX, out uint dpiY);

    [DllImport("shcore.dll")]
    public static extern uint GetDpiForMonitor(nint hmonitor,
        DpiType dpiType,
        out uint dpiX,
        out uint dpiY);

    #endregion

    #region The Windows Shell

    [LibraryImport("Shcore.dll")]
    public static partial int GetScaleFactorForMonitor(
        nint hMonitor,
        ref int scale
    );


    #endregion
}