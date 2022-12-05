/*
  LittleBigMouse.Screen.Config
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of LittleBigMouse.Screen.Config.

    LittleBigMouse.Screen.Config is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    LittleBigMouse.Screen.Config is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media;

//using System.Drawing;

namespace HLab.Base.Wpf.Themes
{
    // TODO: Add a listener for WM_SETTINGCHANGE to detect changes of the active color scheme automatically.
    //   Add a listener for WM_SETTINGCHANGE and trigger an event, like ActiveSetChanged.
    public class AccentColorSet
    {
        public static AccentColorSet[] AllSets
        {
            get
            {
                if (_allSets == null)
                {
                    uint colorSetCount = UXTheme.GetImmersiveColorSetCount();

                    List<AccentColorSet> colorSets = new List<AccentColorSet>();
                    for (uint i = 0; i < colorSetCount; i++)
                        colorSets.Add(new AccentColorSet(i, false));

                    AllSets = colorSets.ToArray();
                }

                return _allSets;
            }
            private set => _allSets = value;
        }

        public static AccentColorSet ActiveSet
        {
            get
            {
                uint activeSet = UXTheme.GetImmersiveUserColorSetPreference(false, false);
                //ActiveSet = AllSets[activeSet];
                ActiveSet = AllSets[activeSet % AllSets.Length];
                return _activeSet;
            }
            private set
            {
                if (_activeSet != null)
                    _activeSet.Active = false;
                value.Active = true;
                _activeSet = value;
            }
        }

        public bool Active { get; private set; }

        public Color this[string colorName]
        {
            get
            {
                IntPtr name = IntPtr.Zero;
                uint colorType;
                try
                {
                    name = Marshal.StringToHGlobalUni("Immersive" + colorName);
                    colorType = UXTheme.GetImmersiveColorTypeFromName(name);
                    if (colorType == 0xFFFFFFFF)
                        throw new InvalidOperationException();
                }
                finally
                {
                    if (name != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(name);
                        name = IntPtr.Zero;
                    }
                }
                return this[colorType];
            }
        }

        public Color this[uint colorType]
        {
            get
            {
                uint nativeColor = UXTheme.GetImmersiveColorFromColorSetEx(_colorSet, colorType, false, 0);
                //if (nativeColor == 0)
                //    throw new InvalidOperationException();
                return Color.FromArgb(
                    (byte)((0xFF000000 & nativeColor) >> 24),
                    (byte)((0x000000FF & nativeColor) >> 0),
                    (byte)((0x0000FF00 & nativeColor) >> 8),
                    (byte)((0x00FF0000 & nativeColor) >> 16)
                    );
            }
        }

        AccentColorSet(uint colorSet, bool active)
        {
            _colorSet = colorSet;
            Active = active;
        }

        static AccentColorSet[] _allSets;
        static AccentColorSet _activeSet;

        uint _colorSet;

        // HACK: GetAllColorNames collects the available color names by brute forcing the OS function.
        //   Since there is currently no known way to retrieve all possible color names,
        //   the method below just tries all indices from 0 to 0xFFF ignoring errors.
        public List<string> GetAllColorNames()
        {
            List<string> allColorNames = new List<string>();
            for (uint i = 0; i < 0xFFF; i++)
            {
                IntPtr typeNamePtr = UXTheme.GetImmersiveColorNamedTypeByIndex(i);
                if (typeNamePtr != IntPtr.Zero)
                {
                    IntPtr typeName = (IntPtr)Marshal.PtrToStructure(typeNamePtr, typeof(IntPtr));
                    allColorNames.Add(Marshal.PtrToStringUni(typeName));
                }
            }
            return allColorNames;
        }

        static class UXTheme
        {
            [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveUserColorSetPreference(bool forceCheckRegistry, bool skipCheckOnFail);

            [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorSetCount();

            [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorFromColorSetEx(uint immersiveColorSet, uint immersiveColorType,
                bool ignoreHighContrast, uint highContrastCacheMode);

            [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorTypeFromName(IntPtr name);

            [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern IntPtr GetImmersiveColorNamedTypeByIndex(uint index);
        }
    }
}
