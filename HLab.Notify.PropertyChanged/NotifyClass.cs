using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged;

public class NotifyClass<TClass>
{
    public class Notifier : INotifyPropertyChanged
    {
        ConcurrentDictionary<NotifyPropertyA, NotifyPropertyA.NotifyPropertyValue> _dict = new ConcurrentDictionary<NotifyPropertyA, NotifyPropertyA.NotifyPropertyValue>();

        public Notifier()
        {

        }
        public TValue Get<TValue>(NotifyPropertyA<TClass,TValue> property) 
            => ((NotifyPropertyA<TClass,TValue>.NotifyPropertyValue)_dict.GetOrAdd((NotifyPropertyA) property,(Func<NotifyPropertyA, NotifyPropertyA.NotifyPropertyValue>) property.GetValue)).Get();
        public void Set<TValue>(NotifyPropertyA<TClass, TValue> property, TValue value)
        {
            if (((NotifyPropertyA<TClass, TValue>.NotifyPropertyValue) _dict.GetOrAdd((NotifyPropertyA) property, (Func<NotifyPropertyA, NotifyPropertyA.NotifyPropertyValue>) property.GetValue))
                .Set(value))
            {
                OnPropertyChanged(property.Name);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public static NotifyClass<TClass> Register() => new NotifyClass<TClass>();
}