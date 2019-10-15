using System;
using System.Collections.Generic;
using System.Text;

namespace HLab.Base.Fluent
{
    public interface IFluentConfigurator<out T>
    {
        T Target { get; }
    }

    public class FluentConfigurator<T> : IFluentConfigurator<T>
    {
        private readonly T _target;
        public FluentConfigurator(T target)
        {
            _target = target;
        }

        T IFluentConfigurator<T>.Target => _target;
    }

    public static class FluentExtension
    {
        public static TC Set<TC,T>(this TC conf, Action<T> action)
        where TC : IFluentConfigurator<T>
        {
            action(conf.Target);
            return conf;
        }
    }
}
