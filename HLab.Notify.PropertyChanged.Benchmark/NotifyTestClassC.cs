using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged.Benchmark
{
    class NotifyTestClassC : INotifyPropertyChanged
    {
        private void Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
            where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(backingField, value))
            {
                backingField = value;
                OnPropertyChanged(propertyName);
            }
        }

        private int _value;
        public int Value
        {
            get => _value;
            set => Set(ref _value,value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}