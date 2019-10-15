using System.ComponentModel;

namespace HLab.Notify.PropertyChanged.Benchmark
{
    class NotifyTestClassA : INotifyPropertyChanged
    {
        private static NotifyClass<NotifyTestClassA> _notifyClass = NotifyClass<NotifyTestClassA>.Register();

        protected NotifyClass<NotifyTestClassA>.Notifier N = new NotifyClass<NotifyTestClassA>.Notifier();

        public static NotifyPropertyA<NotifyTestClassA,int> ValueProperty = NotifyPropertyA<NotifyTestClassA,int>
            .Register(nameof(Value));
        public int Value
        {
            get => ValueProperty.Get(N);
            set => ValueProperty.Set(N,value);
        }


        public event PropertyChangedEventHandler PropertyChanged
        {
            add => N.PropertyChanged += value;
            remove => N.PropertyChanged -= value;
        }
    }
}