using System;
using System.Windows;

namespace HLab.Base.Wpf
{
    public class RoutedEventConfigurator<TClass,TValue>
        where TClass : DependencyObject
    {
        readonly string _name;
        RoutingStrategy _routingStrategy = RoutingStrategy.Tunnel;
        public RoutedEventConfigurator(string name)
        {
            _name = name;
        }

        public RoutedEvent Register() => EventManager.RegisterRoutedEvent(
            _name,
            _routingStrategy,
            typeof(TValue),
            typeof(TClass)
        );

        RoutedEventConfigurator<TClass,TValue> Do(Action action)
        {
            action();
            return this;
        }

        public RoutedEventConfigurator<TClass, TValue> Tunnel => Do(() => _routingStrategy = RoutingStrategy.Tunnel);
        public RoutedEventConfigurator<TClass, TValue> Bubble => Do(() => _routingStrategy = RoutingStrategy.Bubble);
        public RoutedEventConfigurator<TClass, TValue> Direct => Do(() => _routingStrategy = RoutingStrategy.Direct);
    }
}