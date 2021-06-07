using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged.Benchmark
{
    class NotifyTestClassB : INotifyPropertyChanged
    {
        public static NotifyPropertyB<NotifyTestClassB, int> ValueProperty = NotifyPropertyB<NotifyTestClassB, int>
                .Register(c => c._value, (c, v) => c._value = v, c => c.OnPropertyChanged(nameof(Value)))
            //.TriggerOn(()=>ValueProperty)
            ;

        private int _value;
        public int Value
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
