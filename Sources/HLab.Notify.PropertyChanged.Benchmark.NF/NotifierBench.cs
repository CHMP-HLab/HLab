using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HLab.Notify.PropertyChanged.Benchmark.NF.Annotations;

namespace HLab.Notify.PropertyChanged.Benchmark.NF
{
    public class DependencyBench : DependencyObject
    {
        public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
            "PropertyType", typeof(int), typeof(DependencyBench), new PropertyMetadata(default(int)));

        public int Property
        {
            get => (int) GetValue(PropertyProperty);
            set => SetValue(PropertyProperty, value);
        }
    }

    public class DummyBench : INotifyPropertyChanged
    {
        private int _property;

        public int Property
        {
            get => _property;
            set
            {
                if (_property == value) return;
                _property = value;
                OnPropertyChanged();
            }
        }

        private int _propertyLocked;

        public int PropertyLocked
        {
            get => _property;
            set
            {
                var old = Interlocked.Exchange(ref _propertyLocked, value);
                if (old == value) return;
                OnPropertyChanged();
            }
        }

        private void SetValue(ref int target, int value)
        {
            var old = Interlocked.Exchange(ref target, value);
            if (old == value) return;
            OnPropertyChanged();
        }

        private int _propertyLockedHelper;

        public int PropertyLockedHelper
        {
            get => _propertyLockedHelper;
            set => SetValue(ref _propertyLockedHelper,value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class NotifierEmptyBench : N<NotifierEmptyBench>
    {
        public NotifierEmptyBench()
        {
            H.Register(this);
        }
        public int Property {get; set; }
    }

    public class NotifierBench : N<NotifierBench>
    {
        public NotifierBench()
        {
            H.Register(this);
        }

        public int Property
        {
            get => _property.Get();
            set => _property.Set(value);
        }

        private readonly IProperty<int> _property = H.Property<int>();
    }
}
