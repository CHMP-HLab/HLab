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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using HLab.Base;

namespace HLab.Notify.Annotations;

public interface INotifier<TClass> : INotifier
{
    bool SetOneToMany<T>(T value, Func<T, IList<TClass>> getCollection, [CallerMemberName] string propertyName = null);
}
public interface INotifier
{
    INotifier Subscribe();
    Suspender Suspend { get; }
    void Add(PropertyChangedEventHandler value);
    void Remove(PropertyChangedEventHandler value);
    void OnPropertyChanged(PropertyChangedEventArgs args);
    bool Set<T>(T value, [CallerMemberName] string propertyName = null, Action<T, T> postUpdateAction = null);
    T Get<T>(Func<T> getter, [CallerMemberName] string propertyName = null);
    T Get<T>(Func<T, T> getter, [CallerMemberName] string propertyName = null);
    T Locate<T>([CallerMemberName] string propertyName = null);
    T Locate<T>(Func<T, T> func, [CallerMemberName] string propertyName = null);
    void Subscribe(PropertyChangedEventHandler handler, TriggerPath path);
    bool SetOneToMany<TClass,T>(T value, Func<T, IList<TClass>> getCollection, [CallerMemberName] string propertyName = null);

    void Subscribe(INotifierProperty triggeredProperty, INotifierProperty targetProperty, TriggerPath path);
    INotifierPropertyEntry GetPropertyEntry(string propertyName);
    INotifierPropertyEntry<T> GetPropertyEntry<T>(string propertyName);
    INotifierPropertyEntry<T> GetPropertyEntry<T>(PropertyInfo propertyInfo);
    INotifierPropertyEntry GetPropertyEntry(INotifierProperty property);
    INotifierPropertyEntry<T> GetPropertyEntry<T>(INotifierProperty<T> property);
    bool Subscribed { get; }
    INotifyPropertyChanged Target { get; }
}