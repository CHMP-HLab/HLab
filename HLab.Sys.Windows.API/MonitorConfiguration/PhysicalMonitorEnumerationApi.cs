using System.Runtime.InteropServices;

namespace HLab.Sys.Windows.API.MonitorConfiguration;


public static partial class PhysicalMonitorEnumerationApi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PhysicalMonitor
    {
        public nint hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyPhysicalMonitors(
        uint dwPhysicalMonitorArraySize, ref PhysicalMonitor[] pPhysicalMonitorArray);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(nint hMonitor, ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    static extern bool GetPhysicalMonitorsFromHMONITOR(nint hMonitor, uint dwPhysicalMonitorArraySize,
        [Out] PhysicalMonitor[] pPhysicalMonitorArray);

    public static PhysicalMonitor[] GetPhysicalMonitorsFromHMONITOR(nint hMonitor)
    {
        uint pdwNumberOfPhysicalMonitors = 0;

        if (GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref pdwNumberOfPhysicalMonitors))
        {
            var pPhysicalMonitorArray = new PhysicalMonitor[pdwNumberOfPhysicalMonitors];

            if (GetPhysicalMonitorsFromHMONITOR(hMonitor, pdwNumberOfPhysicalMonitors, pPhysicalMonitorArray))
            {
                return pPhysicalMonitorArray;
            }
        }

        return null;
    }
}