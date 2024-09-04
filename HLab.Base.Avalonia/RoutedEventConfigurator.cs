using Avalonia;
using Avalonia.Interactivity;

namespace HLab.Base.Avalonia;

public class RoutedEventConfigurator<TClass,TValue>
    where TClass : AvaloniaObject where TValue : RoutedEventArgs
{
    readonly string _name;
    RoutingStrategies _routingStrategy = RoutingStrategies.Tunnel;
    public RoutedEventConfigurator(string name)
    {
        _name = name;
    }

    public RoutedEvent Register() => RoutedEvent.Register<TClass,TValue>( //EventManager.RegisterRoutedEvent(
        _name,
        _routingStrategy
    );

    RoutedEventConfigurator<TClass,TValue> Do(Action action)
    {
        action();
        return this;
    }

    public RoutedEventConfigurator<TClass, TValue> Tunnel => Do(() => _routingStrategy |= RoutingStrategies.Tunnel);
    public RoutedEventConfigurator<TClass, TValue> Bubble => Do(() => _routingStrategy |= RoutingStrategies.Bubble);
    public RoutedEventConfigurator<TClass, TValue> Direct => Do(() => _routingStrategy |= RoutingStrategies.Direct);
}