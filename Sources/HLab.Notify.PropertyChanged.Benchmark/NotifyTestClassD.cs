using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace HLab.Notify.PropertyChanged.Benchmark
{


    class NotifyTestClassD : IOnPropertyChanged
    {
        private Property<int> _value = Property<int>.Register(nameof(Value));
        public int Value
        {
            get => _value.Get();
            set => _value.Set(this,value);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
        PropertyChanged?.Invoke(this, args);
        }
    }

}
