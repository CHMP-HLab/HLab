using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace HLab.Notify.Collections
{
    public static class ObservableCollectionExt
    {
        public static void Sort<T>(this ObservableCollectionNotifier<T> collection, Comparison<T> comparison)
            where T : INotifyPropertyChanged
        {
            var list = new List<T>(collection);
            list.Sort(comparison);

            for (int i = 0; i < list.Count; i++)
            {
                if (collection.IndexOf(list[i]) == i) continue;
                collection.Remove(list[i]);
                collection.Insert(i,list[i]);
            }
        }

        public static T SetObserver<T>(this T  collection, NotifyCollectionChangedEventHandler handler)
            where T : INotifyCollectionChanged
        {
            //var l = collection as ILockable;

            //l?.Lock.EnterWriteLock(); 
            try
            {
                handler?.Invoke(null,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        ((IList) collection).OfType<object>().ToList(),
                        0));
                collection.CollectionChanged += handler;
                //return this;
            }
            finally
            {
                //l?.Lock.ExitWriteLock();
            }
            return collection;
        }
    }
}