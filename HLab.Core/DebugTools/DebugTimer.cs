/*
  HLab.Base
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

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
#define TIMER

using System;
using System.Diagnostics;

namespace HLab.Core.DebugTools;

public class DebugTimer : IDisposable
{
    IDebugLogger _log;
#if TIMER
    readonly Stopwatch _sw;
    readonly string _name;
#endif

    public DebugTimer(string name, IDebugLogger log)
    {
#if TIMER
        _name = name;
        _log = log;
        _sw = new Stopwatch();
        _sw.Start();
#endif
    }

    public void Dispose()
    {
#if TIMER
        _sw.Stop();
        _log.Log(_name,_sw);
#endif
    }
}