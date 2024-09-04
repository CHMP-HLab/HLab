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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HLab.Notify.Annotations;

public static class NotifierExt
{


    public static T Get<T>(this INotifier n,
        [CallerMemberName] string propertyName = null)
        => n.Get<T>(
            old => default(T), 
            propertyName);

    public static T Get<T>(this INotifier n, Func<T> getter,
        [CallerMemberName] string propertyName = null)
        => n.Get<T>(old => getter(), propertyName);

    public static T Get<T>(this INotifier n, Func<T, T> getter,
        [CallerMemberName] string propertyName = null)
        => n.Get<T>(getter, propertyName);

    public static bool Set<T>(this INotifier n,
        T value,
        Action<T, T> postUpdateAction,
        [CallerMemberName] string propertyName = null)
        => n.Set(value, propertyName, postUpdateAction);

    public static bool Set<T>(this INotifier n,
        T value,
        [CallerMemberName] string propertyName = null)
        => n.Set(value, propertyName, null);

    //public static void SubscribeNotifier(this INotifyPropertyChanged n) => n.GetNotifier().Subscribe();


    public static void UnSubscribeNotifier(this INotifyPropertyChanged n, PropertyChangedEventHandler action,
        IList<string> targets)
    {
        if (PropertiesHandlers.TryRemove(Tuple.Create(action, targets), out PropertyChangedEventHandler h))
        {
            if (targets.Count > 1)
            {
                if (n.GetType().GetProperty(targets[0])?.GetValue(n) is INotifyPropertyChanged value)
                {
                    value.UnSubscribeNotifier(action, targets.Skip(1).ToArray());
                }
            }
            n.PropertyChanged -= h;
        }
    }
    public static void UnSubscribeNotifier(this INotifyCollectionChanged n, PropertyChangedEventHandler action,
        IList<string> targets)
    {
        if (CollectionHandlers.TryRemove(Tuple.Create(action, targets), out NotifyCollectionChangedEventHandler h))
        {
            if (targets.Count > 1)
            {
                var newTargets = targets.Skip(1).ToArray();

                if (n is IList oldItems)
                    foreach (var item in oldItems)
                    {
                        UnSubscribeNotifier(item,action,newTargets);
                        action(n, new NotifierPropertyChangedEventArgs("Item", item, null));
                    }
            }
            n.CollectionChanged -= h;
        }
    }

    public static void UnSubscribeNotifier(object n, PropertyChangedEventHandler action,
        IList<string> targets)
    {
        if (n is INotifyCollectionChanged oldCollection)
            oldCollection.UnSubscribeNotifier(action, targets);

        if (n is INotifyPropertyChanged oldValue)
            oldValue.UnSubscribeNotifier(action, targets);
    }

    static readonly
        ConcurrentDictionary<Tuple<PropertyChangedEventHandler, IList<string>>, PropertyChangedEventHandler>
        PropertiesHandlers =
            new ConcurrentDictionary<Tuple<PropertyChangedEventHandler, IList<string>>,
                PropertyChangedEventHandler>();

    static readonly
        ConcurrentDictionary<Tuple<PropertyChangedEventHandler, IList<string>>, NotifyCollectionChangedEventHandler>
        CollectionHandlers =
            new ConcurrentDictionary<Tuple<PropertyChangedEventHandler, IList<string>>,
                NotifyCollectionChangedEventHandler>();

    public static void Unsubscribe(this object value, IList<string> targets, PropertyChangedEventHandler handler)
    {
        if (value is INotifyCollectionChanged oldCollection && targets[0] == "Item")
        {
            oldCollection.UnSubscribeNotifier(handler, targets);
            return;
        }

        if (value is INotifyPropertyChanged oldNotifier)
            oldNotifier.UnSubscribeNotifier(handler, targets);
    }

    //public static T Subscribe<T>(this T obj) where T : INotifierObject
    //{
    //    obj.GetNotifier().Subscribe();
    //    return obj;
    //}


    static void Subscribe(this object value, IList<string> path, PropertyChangedEventHandler handler)
    {
        switch (value)
        {
            case INotifyCollectionChanged newCollection when path[0] == "Item":
                newCollection.SubscribeNotifier(path, handler);
                return;
            case INotifierObject newNotifier:
                newNotifier.GetNotifier().Subscribe(path, handler);
                return;
            default:
                throw new ArgumentException("object : " + value.GetType() + " does not implement INotifyPropertyChanged" );
        }
    }



    public static void SubscribeNotifier(
        this INotifyCollectionChanged n,
        IList<string> path,
        PropertyChangedEventHandler handler)
    {
        Debug.Assert(path[0] == "Item");
        NotifyCollectionChangedEventHandler collectionHandler;


        if(path.Count<2)
            collectionHandler = (sender, args) =>
            {
                if (args.NewItems != null)
                    foreach (var item in args.NewItems)
                    {
                        handler(sender, new NotifierPropertyChangedEventArgs("Item", null, item));
                    }

                if (args.OldItems != null)
                    foreach (var item in args.OldItems)
                    {
                        handler(sender, new NotifierPropertyChangedEventArgs("Item", item, null));
                    }
            };
        else
        {
            var newPath = path.Skip(1).ToList();

            collectionHandler = (sender, args) =>
            {
                if (args.NewItems != null)
                    foreach (var item in args.NewItems)
                    {
                        item.Subscribe(newPath, handler);
                        handler(sender, new NotifierPropertyChangedEventArgs("Item", null, item));
                    }

                if (args.OldItems != null)
                    foreach (var item in args.OldItems)
                    {
                        item.Unsubscribe(newPath, handler);
                        handler(sender, new NotifierPropertyChangedEventArgs("Item", item, null));
                    }
            };
        }


        if (n is IList newItems)
        {
            collectionHandler(null,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, 0));

        }

        //CollectionHandlers.TryAdd(Tuple.Create(action, targets), H);
        n.CollectionChanged += collectionHandler;
    }

    //public static void SubscribeOneToMany<T>(
    //    this object target, 
    //    IList<T> list,
    //    string property)
    //{
    //    NotifierService.D.GetNotifierClass(typeof(T)).GetProperty(property).RegisterOneToMany(target,(IList)list);
    //}
}