﻿/*
  HLab.Mvvm
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Mvvm.

    HLab.Mvvm is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Mvvm is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;


////using System.Data.Model;

namespace HLab.Mvvm.Annotations
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(double progress, string progressText)
        {
            Progress = progress;
            ProgressText = progressText;
        }

        public double Progress { get; }
        public string ProgressText { get; }
    }
}
