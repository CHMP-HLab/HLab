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

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace HLab.Sys.Windows.API.MonitorConfiguration;

public static partial class HighLevelMonitorConfigurationApi
{
    [Flags]
    public enum MonitorCapabilities : uint
    {
        /// <summary>
        /// The monitor does not support any monitor settings.
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// The monitor supports the GetMonitorTechnologyType function.
        /// </summary>
        MonitorTechnologyType = 0x00000001,
        /// <summary>
        /// The monitor supports the GetMonitorBrightness and SetMonitorBrightness functions.
        /// </summary>
        Brightness = 0x00000002,
        /// <summary>
        /// The monitor supports the GetMonitorContrast and SetMonitorContrast functions.
        /// </summary>
        Contrast = 0x00000004,
        /// <summary>
        /// The monitor supports the GetMonitorColorTemperature and SetMonitorColorTemperature functions.
        /// </summary>
        ColorTemperature = 0x00000008,
        /// <summary>
        /// The monitor supports the GetMonitorRedGreenOrBlueGain and SetMonitorRedGreenOrBlueGain functions.
        /// </summary>
        RedGreenBlueGain = 0x00000010,
        /// <summary>
        /// The monitor supports the GetMonitorRedGreenOrBlueDrive and SetMonitorRedGreenOrBlueDrive functions.
        /// </summary>
        RedGreenBlueDrive = 0x00000020,
        /// <summary>
        /// The monitor supports the DegaussMonitor function.
        /// </summary>
        Degauss = 0x00000040,
        /// <summary>
        /// The monitor supports the GetMonitorDisplayAreaPosition and SetMonitorDisplayAreaPosition functions.
        /// </summary>
        DisplayAreaPosition = 0x00000080,
        /// <summary>
        /// The monitor supports the GetMonitorDisplayAreaSize and SetMonitorDisplayAreaSize functions.
        /// </summary>
        DisplayAreaSize = 0x00000100,
        /// <summary>
        /// The monitor supports the RestoreMonitorFactoryDefaults function.
        /// </summary>
        RestoreFactoryDefaults = 0x00000400,
        /// <summary>
        /// The monitor supports the RestoreMonitorFactoryColorDefaults function.
        /// </summary>
        RestoreFactoryColorDefaults = 0x00000800,
        /// <summary>
        /// If this flag is present, calling the RestoreMonitorFactoryDefaults function enables all of the monitor settings used by the high-level monitor configuration functions. For more information, see the Remarks section in RestoreMonitorFactoryDefaults.
        /// </summary>
        RestoreFactoryDefaultsEnablesMonitorSettings = 0x00001000,
    }

    [Flags]
    public enum MonitorSupportedColorTemperatures : uint
    {
        None = 0x00000000,
        T4000K = 0x00000001,
        T5000K = 0x00000002,
        T6500K = 0x00000004,
        T7500K = 0x00000008,
        T8200K = 0x00000010,
        T9300K = 0x00000020,
        T10000K = 0x00000040,
        T11500K = 0x00000080,
    }


    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorCapabilities(
        nint hMonitor,
        [Out] out MonitorCapabilities monitorCapabilities,
        [Out] out MonitorSupportedColorTemperatures supportedColorTemperatures);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorBrightness(nint hMonitor, ref uint pdwMinimumBrightness, ref uint pdwCurrentBrightness, ref uint pdwMaximumBrightness);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool SetMonitorBrightness(nint hMonitor, uint dwNewBrightness);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorContrast(nint hMonitor, ref uint pdwMinimumContrast, ref uint pdwCurrentContrast, ref uint pdwMaximumContrast);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool SetMonitorContrast(nint hMonitor, uint dwNewContrast);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorRedGreenOrBlueGain(nint hMonitor, uint component, ref uint pdwMinimumContrast, ref uint pdwCurrentContrast, ref uint pdwMaximumContrast);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool SetMonitorRedGreenOrBlueGain(nint hMonitor, uint component, uint dwNewContrast);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorRedGreenOrBlueDrive(nint hMonitor, uint component, ref uint pdwMinimumContrast, ref uint pdwCurrentContrast, ref uint pdwMaximumContrast);

    [DllImport("Dxva2.dll", CharSet = CharSet.Auto)]
    public static extern bool SetMonitorRedGreenOrBlueDrive(nint hMonitor, uint component, uint dwNewContrast);

}