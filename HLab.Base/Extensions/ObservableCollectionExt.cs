using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HLab.Base.Extensions
{
    public static class ObservableCollectionExt
    {
        public static void Sort<T>(this IList<T> collection, Comparison<T> comparison)
            where T : INotifyPropertyChanged
        {
            var list = new List<T>(collection);
            list.Sort(comparison);

            for (int i = 0; i < list.Count; i++)
            {
                if (collection.IndexOf(list[i]) == i) continue;
                collection.Remove(list[i]);
                collection.Insert(i, list[i]);
            }
        }

        public static T SetObserver<T>(this T collection, NotifyCollectionChangedEventHandler handler)
            where T : INotifyCollectionChanged
        {
            if((collection is ILockable l)) l.Lock.EnterReadLock();
            try
            {
                if (collection is IList list)
                {
                    if (list.Count > 0)
                    {
                        handler?.Invoke(null,
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add,
                                list.OfType<object>().ToList(),
                                0));
                        collection.CollectionChanged += handler;
                    }
                }
                return collection;
            }
            finally
            {
                if(collection is ILockable l2 && l2.Lock.IsReadLockHeld) l2.Lock.ExitReadLock();
            }
        }

        public static T GetOrAdd<T>(this IList<T> col, Func<T, bool> comparator, Func<T> getter)
        {
            if (col is ILockable l) l.Lock.EnterWriteLock();
            try
            {
                foreach (var item in col)
                {
                    if (comparator(item)) return item;
                }

                var newItem = getter();

                col.Add(newItem);

                return newItem;
            }
            finally
            {
                if (col is ILockable l2 && l2.Lock.IsWriteLockHeld) l2.Lock.ExitWriteLock();
            }
        }
    }
}
