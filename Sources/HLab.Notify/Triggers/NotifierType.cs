/*
  HLab.Notify.4
  Copyright (c) 2017 Mathieu GRENET.  All right reserved.

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
using HLab.Notify.Annotations;

namespace HLab.Notify.Triggers
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotifierTypeAttribute : Attribute
    {
        private readonly Type _type;
        public NotifierTypeAttribute(Type type)
        {
            _type = type;
        }

        public INotifierPropertyEntry GetEntry()
        {
            var entry = (INotifierPropertyEntry)Activator.CreateInstance(_type);


            return entry;
        }
    }




}
