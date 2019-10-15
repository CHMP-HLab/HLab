using System;

namespace HLab.Notify5
{
    public class ReactiveProperty<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
