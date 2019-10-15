using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged.Benchmark
{
    class NotifyTestClass1 : INotifyPropertyChanged
    {
        private int _value;
        public int Value
        {
            get => _value;
            set {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}