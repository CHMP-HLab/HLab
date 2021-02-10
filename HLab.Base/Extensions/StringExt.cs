/*
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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HLab.Base.Extensions
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

        public static string FromCamelCase(this string s)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
            return r.Replace(s," ");
        }

        public static string BeforeSuffix(this string s, string suffix)
        {
            var i = s.IndexOf(suffix);
            if (i >= 0) return s.Substring(0, i);
            return s;
        }
        public static IEnumerable<string> GetInside(this string source,char left, char right)
        {
            var s = "";
            var level = 0;
            foreach (var c in source)
            {
                if(c==right)
                {
                    level--;
                    if (level != 0) continue;
                    yield return s;
                    s = "";
                }
                else if(c==left) level++;
                else if (level>0) s += c;
            }

            if (level != 0) throw new ArgumentException("Invalid expression");
        }

    }
}
