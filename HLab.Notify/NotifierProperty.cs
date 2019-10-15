using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;

namespace HLab.Notify
{



    public class NotifierProperty<TClass,T> : INotifierProperty<T>
    {
        private NotifierClass<TClass> _class;
        public string Name { get; private set; }
        //public NotifierProperty(NotifierClass @class, string name, Func<INotifyPropertyChanged, INotifierProperty, NotifierPropertyEntry> getNewEntry)
        //{
        //    _class = @class;
        //    Name = name;
        //    _getNewEntry = getNewEntry;
        //}
        public NotifierProperty(NotifierClass<TClass> cls, string name)
        {
            _class = cls;
            Name = name;
        }

        [Import]
        private readonly Func<INotifyPropertyChanged, INotifierProperty<T>, NotifierPropertyEntry<T>> _getNewEntry;

        private readonly ConditionalWeakTable<object, IList> _weakOneToMany = new ConditionalWeakTable<object, IList>();

        INotifierPropertyEntry<T> INotifierProperty<T>.GetNewEntry(INotifyPropertyChanged notifier) => GetNewEntry(notifier);
        INotifierPropertyEntry INotifierProperty.GetNewEntry(INotifyPropertyChanged notifier) => GetNewEntry(notifier);
        public NotifierPropertyEntry<T> GetNewEntry(INotifyPropertyChanged notifier)
        {
            return _getNewEntry(notifier, this);
        }

        public void RegisterOneToMany(object target, IList list)
        {
            _weakOneToMany.Add(target, list);
        }

        public void AddOneToMany(T oldValue, T newValue, object target)
        {

            if (oldValue!=null && _weakOneToMany.TryGetValue(oldValue, out var oldCollection))
            {
                oldCollection.Remove(target);
            }
            if (newValue!=null && _weakOneToMany.TryGetValue(newValue, out var newCollection))
            {
                newCollection.Add(target);
            }
        }
    }
}
