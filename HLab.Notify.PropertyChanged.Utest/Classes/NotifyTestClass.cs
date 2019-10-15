using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged.UTest
{
    class NotifyProperty
    {
        internal class NotifyPropertyValue
        {

        }

    }

    class NotifyProperty<TClass,TValue> : NotifyProperty
    {
        internal new class NotifyPropertyValue : NotifyProperty.NotifyPropertyValue
        {
            private TValue _value;
            public TValue Get() => _value;
            public bool Set(TValue value)
            {
                if (Equals(_value, value)) return false;
                _value = value;
                return true;
            }
        }

        private NotifyProperty(string name)
        {
            Name = name;
        }
        private NotifyProperty(Func<TClass, TValue> getter, Action<TClass, TValue> setter, Action<TClass> change)
        {
            _getter = getter;
            _setter = setter;
            _onPropertyChanged = change;
        }

        public string Name { get; }
        private readonly Func<TClass, TValue> _getter;
        private readonly Action<TClass, TValue> _setter;
        private readonly Action<TClass> _onPropertyChanged;

        public static NotifyProperty<TClass, TValue> Register(string name) => new NotifyProperty<TClass, TValue>(name);
        public static NotifyProperty<TClass, TValue> Register(Func<TClass,TValue> getter, Action<TClass,TValue> setter, Action<TClass> change) 
            => new NotifyProperty<TClass, TValue>(getter,setter,change);

        public TValue Get(NotifyClass<TClass>.Notifier n) => n.Get(this);
        public void Set(NotifyClass<TClass>.Notifier n, TValue value) => n.Set(this, value);
        public TValue Get(TClass c) => _getter(c);
        public void Set(TClass c, TValue value)
        {
            var oldValue = _getter(c);
            if (!Equals(oldValue, value))
            {
                _setter(c, value);
                _onPropertyChanged(c);
            }
        }

        public NotifyPropertyValue GetValue(NotifyProperty property)
        {
            return new NotifyPropertyValue();
        }
    }

    class NotifyClass<TClass>
    {
        internal class Notifier : INotifyPropertyChanged
        {
            private ConcurrentDictionary<NotifyProperty, NotifyProperty.NotifyPropertyValue> _dict = new ConcurrentDictionary<NotifyProperty, NotifyProperty.NotifyPropertyValue>();

            public Notifier()
            {

            }
            public TValue Get<TValue>(NotifyProperty<TClass,TValue> property) 
                => ((NotifyProperty<TClass,TValue>.NotifyPropertyValue)_dict.GetOrAdd(property,property.GetValue)).Get();
            public void Set<TValue>(NotifyProperty<TClass, TValue> property, TValue value)
            {
                if (((NotifyProperty<TClass, TValue>.NotifyPropertyValue) _dict.GetOrAdd(property, property.GetValue))
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


    class NotifyTestClassB : INotifyPropertyChanged
    {
        public static NotifyProperty<NotifyTestClassB, string> ValueProperty = NotifyProperty<NotifyTestClassB, string>
                .Register(c => c._value, (c, v) => c._value = v, c => c.OnPropertyChanged(nameof(Value)))
            //.TriggerOn(()=>ValueProperty)
            ;

        private string _value;
        public string Value
        {
            get => ValueProperty.Get(this);
            set => ValueProperty.Set(this, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


}
