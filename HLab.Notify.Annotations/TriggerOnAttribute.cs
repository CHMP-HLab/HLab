﻿/*
  HLab.Notify.4
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Notify.4.

    HLab.Notify.4 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Notify.4 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;
using System.Linq.Expressions;

namespace HLab.Notify.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class TriggerOnAttribute : Attribute
    {
        public TriggerOnAttribute()
        {
            Path = null;
        }

        public TriggerOnAttribute(params string[] path)
        {
            Path = TriggerPath.Factory(path);
        }

        public TriggerPath Path { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class TriggerOnAttribute<T> : Attribute
    {
        public TriggerOnAttribute()
        {
            Path = null;
        }

        public TriggerOnAttribute(Expression<Func<T, object>> expr)
        {
            Path = TriggerPath.Factory(expr);
        }

        public TriggerPath Path { get; }
    }
}
