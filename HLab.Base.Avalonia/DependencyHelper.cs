using System.Runtime.CompilerServices;

namespace HLab.Base.Avalonia
{
    public class DependencyHelper
    {
        public static DependencyPropertyConfigurator<TClass, TValue> Property<TClass, TValue>([CallerMemberName] string name = null)
            where TClass : StyledObject
        {
            if (name == null) throw new NullReferenceException();

            if (name.EndsWith("Property")) name = name.Substring(0, name.Length - 8);
            return new DependencyPropertyConfigurator<TClass, TValue>(name);
        }
        public static RoutedEventConfigurator<TClass, TValue> Event<TClass, TValue>([CallerMemberName] string name = null)
            where TClass : DependencyObject
        {
            if (name == null) throw new NullReferenceException();

            if (name.EndsWith("Event")) name = name.Substring(0, name.Length - 5);
            return new RoutedEventConfigurator<TClass, TValue>(name);
        }
    }


    public class DependencyHelper<TClass> : DependencyHelper
    where TClass : StyledObject
    {
        public static DependencyPropertyConfigurator<TClass, TValue> Property<TValue>([CallerMemberName] string name = null)
            => Property<TClass, TValue>(name);
        public static RoutedEventConfigurator<TClass, TValue> Event<TValue>([CallerMemberName] string name = null)
            => Event<TClass, TValue>(name);
        public static RoutedEventConfigurator<TClass, RoutedEventHandler> Event([CallerMemberName] string name = null)
            => Event<TClass, RoutedEventHandler>(name);
    }
}
