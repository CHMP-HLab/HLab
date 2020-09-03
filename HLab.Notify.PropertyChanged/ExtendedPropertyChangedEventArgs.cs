using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public class ExtendedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public ExtendedPropertyChangedEventArgs(PropertyChangedEventArgs source, object oldValue, object newValue):base(source.PropertyName)
        {
            Source = source;
            OldValue = oldValue;
            NewValue = newValue;
        }
        public ExtendedPropertyChangedEventArgs(string propertyName, object oldValue, object newValue):base(propertyName)
        {
            Source = null;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public PropertyChangedEventArgs Source { get; }
        public object OldValue { get; }
        public object NewValue { get; }
    }
}