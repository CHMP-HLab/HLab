using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace HLab.Notify.PropertyChanged
{
    public class Notifier : INotifyPropertyChanged
    {
        private readonly ConcurrentDictionary<string, object> _dict = new ConcurrentDictionary<string, object>();

        public TValue Get<TValue>([CallerMemberName]string property = null)
            => Get(()=>default(TValue),property);
        public TValue Get<TValue>(Func<TValue> ctor, [CallerMemberName]string property=null)
            => (TValue)_dict.GetOrAdd(property,n => ctor());
        public bool Set<TValue>(TValue value, [CallerMemberName]string property = null)
        {
                bool update = false;
                _dict.AddOrUpdate(property, value, (s, o) =>
                {
                    update = true;
                    return value;
                });

                if(update) { OnPropertyChanged(property);}

                return update;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
