using System.ComponentModel;

namespace HLab.Notify.Annotations;
//public class ExtendedPropertyChangedEventArgs<T> : ExtendedPropertyChangedEventArgs
//{
//    public ExtendedPropertyChangedEventArgs(PropertyChangedEventArgs source, T oldValue, T newValue) : base(source.PropertyName)
//    {
//        OldValue = oldValue;
//        NewValue = newValue;
//    }
//    public ExtendedPropertyChangedEventArgs(string propertyName, T oldValue, T newValue) : base(propertyName)
//    {
//        OldValue = oldValue;
//        NewValue = newValue;
//    }

//    public override T OldValue { get; }
//    public override T NewValue { get; }
//}

public class ExtendedPropertyChangedEventArgs : PropertyChangedEventArgs
{
    public ExtendedPropertyChangedEventArgs(PropertyChangedEventArgs source, object oldValue, object newValue) : base(source.PropertyName)
    {
        Source = source;
        OldValue = oldValue;
        NewValue = newValue;
    }
    public ExtendedPropertyChangedEventArgs(string propertyName, object oldValue, object newValue) : base(propertyName)
    {
        Source = null;
        OldValue = oldValue;
        NewValue = newValue;
    }
    public ExtendedPropertyChangedEventArgs(string propertyName) : base(propertyName)
    {

    }
    public ExtendedPropertyChangedEventArgs(PropertyChangedEventArgs source) : base(source.PropertyName)
    {

    }

    public PropertyChangedEventArgs Source { get; }
    public /*abstract*/ object OldValue { get; }
    public /*abstract*/ object NewValue { get; }
}