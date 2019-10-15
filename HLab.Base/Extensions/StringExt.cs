﻿/*
  HLab.Base
  Copyright (c) 2017 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Base.

    HLab.Base is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Base is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/
namespace HLab.Base.Extentions
{
    public static class StringExt
    {
        public static bool Like(this string s, string pattern)
        {
            string[] parts = pattern.Split('*');
            if (!s.StartsWith(parts[0])) return false;

            s = s.Remove(0,parts[0].Length);
            if (s == "") return true;

            for (int i = 1; i < parts.Length-1; i++)
            {
                int pos = s.IndexOf(parts[i]);
                if (pos == -1) return false;

                s = s.Remove(0, pos+parts[i].Length);
            }

            string end = parts[parts.Length - 1];

            if (s.EndsWith(end)) return true;
            return false;
        }

        /// <summary>
        /// Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }
}
