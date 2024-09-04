using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace HLab.Base.Extensions;

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
        using((collection as ILockable)?.Lock.ReaderLock())
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
    }

    public static T GetOrAdd<T>(this IList<T> col, Func<T, bool> comparator, Func<T> getter)
    {
        using((col as ILockable)?.Lock.WriterLock())
        {
            foreach (var item in col)
            {
                if (comparator(item)) return item;
            }

            var newItem = getter();

            col.Add(newItem);

            return newItem;
        }
    }
}