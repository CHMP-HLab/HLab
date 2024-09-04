using System;
using System.Runtime.CompilerServices;
using HLab.Notify.Annotations;

namespace HLab.Notify;

public static class NotifierObjectExt
{
    public static INotifier GetNotifier(this INotifierObject obj)
        => obj.GetNotifier();

    public static T Get<T>(this INotifierObject n,
        [CallerMemberName] string propertyName = null)
        => n.GetNotifier().Get<T>(old => default(T), propertyName);

    //public static T Get<T>(this INotifierObject n, Func<T> getter,
    //    [CallerMemberName] string propertyName = null)
    //    => n.GetNotifier().Get<T>(old => getter(), propertyName);

    public static T Get<T>(this INotifierObject n, Func<T, T> getter,
        [CallerMemberName] string propertyName = null)
        => n.GetNotifier().Get<T>(getter, propertyName);

    public static bool Set<T>(this INotifierObject n,
        T value,
        Action<T, T> postUpdateAction,
        [CallerMemberName] string propertyName = null)
        => n.GetNotifier().Set(value, propertyName, postUpdateAction);

    //public static bool Set<T>(this INotifierObject n,
    //    T value,
    //    [CallerMemberName] string propertyName = null)
    //    => n.GetNotifier().Set(value, propertyName, null);

    //public static bool SetOneToMany<T,TNotifier>(this TNotifier n,
    //    T value,
    //    Func<T,IList<TNotifier>> getCollection,
    //    [CallerMemberName] string propertyName = null)
    //    where TNotifier : INotifierObject

    //    => n.GetNotifier().SetOneToMany(n, value, getCollection, propertyName);

    //public static void Subscribe(this INotifierObject n) => n.Notifier.Subscribe(n);
}