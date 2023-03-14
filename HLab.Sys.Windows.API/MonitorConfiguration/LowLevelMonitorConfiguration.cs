using System.Runtime.InteropServices;
using System.Text;

namespace HLab.Sys.Windows.API.MonitorConfiguration;

public static partial class LowLevelMonitorConfiguration
{
    [DllImport("dxva2.dll", EntryPoint = "GetCapabilitiesStringLength", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCapabilitiesStringLength(
        [In] nint hMonitor, ref uint pdwLength);

    [DllImport("dxva2.dll", EntryPoint = "CapabilitiesRequestAndCapabilitiesReply", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool CapabilitiesRequestAndCapabilitiesReply(
        [In] nint hMonitor, StringBuilder pszString, uint dwLength);

    public static bool GetCapabilitiesString(nint hMonitor, out string capabilities)
    {
        uint length = 0;
        if (GetCapabilitiesStringLength(hMonitor, ref length))
        {
            var sb = new StringBuilder((int)length);
            if (CapabilitiesRequestAndCapabilitiesReply(hMonitor, sb, length))
            {
                capabilities = sb.ToString();
                return true;
            }
        }
        capabilities = string.Empty;
        return false;
    }


    [DllImport("dxva2.dll", EntryPoint = "GetVCPFeatureAndVCPFeatureReply", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetVCPFeatureAndVCPFeatureReply(
        [In] nint hMonitor, [In] uint dwVCPCode, out uint pvct, out uint pdwCurrentValue, out uint pdwMaximumValue);


    [DllImport("dxva2.dll", EntryPoint = "SetVCPFeature", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetVCPFeature(
        [In] nint hMonitor, uint dwVCPCode, uint dwNewValue);

}