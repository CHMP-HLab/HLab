using System;

namespace HLab.Base.Fluent;

public delegate void FluentConfiguratorDelegate<in T>(IFluentConfigurator<T> configurator); 

public interface IFluentConfigurator<out T>
{
    T Target { get; }
}

public class FluentConfigurator<T> : IFluentConfigurator<T>
{
    public FluentConfigurator(T target)
    {
        Target = target;
    }

    public T Target { get; }
}

public static class FluentExtension
{
    public static TC Set<TC,T>(this TC conf, Action<T> action)
        where TC : IFluentConfigurator<T>
    {
        action(conf.Target);
        return conf;
    }

    public static T Fluently<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }
}