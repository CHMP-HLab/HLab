/*
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
using System.ComponentModel;

namespace HLab.Notify.Annotations;

public class NotifierPropertyChangedEventArgs : PropertyChangedEventArgs
{
    public INotifierPropertyEntry Entry { get; }
    public object OldValue { get; }
    public object NewValue { get; set; }
    public NotifierPropertyChangedEventArgs(INotifierPropertyEntry entry, object oldValue, object newValue):base(entry.Property.Name)
    {
        Entry = entry;
        OldValue = oldValue;
        NewValue = newValue;
    }
    public NotifierPropertyChangedEventArgs(string propertyName, object oldValue, object newValue) : base(propertyName)
    {
        Entry = null;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public override string ToString()
    {
        var name = Entry.Property.Name;
        string oldValue = "?";
        string newValue = "?";

        try { 
            oldValue = OldValue?.ToString() ?? "";
            newValue = NewValue?.ToString() ?? "";
        }
        catch(InvalidOperationException)
        { }

        return name + " ( " + oldValue + " , " + newValue + " )";
    }
}