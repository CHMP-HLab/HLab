using System.Runtime.InteropServices;
using System.Windows;

namespace HLab.Base.Wpf
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public readonly struct DependencyPropertyChangedEventArgs<TValue> 
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> class.</summary>
        /// <param name="eventArgs"></param>
        public DependencyPropertyChangedEventArgs(
            DependencyPropertyChangedEventArgs eventArgs)
        {
            Property = eventArgs.Property;
            OldValue = (TValue)eventArgs.OldValue;
            NewValue = (TValue)eventArgs.NewValue;

        }


        /// <summary>Gets the value of the property after the change.</summary>
        /// <returns>The property value after the change.</returns>
        public TValue NewValue {get; }

        /// <summary>Gets the value of the property before the change.</summary>
        /// <returns>The property value before the change.</returns>
        public TValue OldValue { get; }

        /// <summary>Gets the identifier for the dependency property where the value change occurred.</summary>
        /// <returns>The identifier field of the dependency property where the value change occurred.</returns>
        public DependencyProperty Property { get; }

        /// <summary>Determines whether the provided object is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</summary>
        /// <param name="obj">The object to compare to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</param>
        /// <returns>
        /// <see langword="true" /> if the provided object is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj) => Equals((DependencyPropertyChangedEventArgs<TValue>) obj);

        /// <summary>Determines whether the provided <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</summary>
        /// <param name="args">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /></param>
        /// <returns>
        /// <see langword="true" /> if the provided <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />; otherwise, <see langword="false" />.</returns>
        public bool Equals(DependencyPropertyChangedEventArgs<TValue> args) =>
            Property.Equals(args.Property) && OldValue.Equals(args.OldValue) && NewValue.Equals(args.NewValue);

        /// <summary>Gets a hash code  for this <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</summary>
        /// <returns>A signed 32-bit integer hash code.</returns>
        public override int GetHashCode() => HashCode.Start.Add(Property).Add(OldValue).Add(NewValue).Value;

        /// <summary>Determines whether two specified <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> objects have the same value.</summary>
        /// <param name="left">The first <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
        /// <param name="right">The second <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if the two <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> instances are equivalent; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(
            DependencyPropertyChangedEventArgs<TValue> left,
            DependencyPropertyChangedEventArgs<TValue> right) => left.Equals(right);

        /// <summary>Determines whether two specified <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> objects are different.</summary>
        /// <param name="left">The first <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
        /// <param name="right">The second <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if the two <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> instances are different; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(
            DependencyPropertyChangedEventArgs<TValue> left,
            DependencyPropertyChangedEventArgs<TValue> right) =>  ! left.Equals(right);
    }
}