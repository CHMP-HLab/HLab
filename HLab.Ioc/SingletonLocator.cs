using System;
using System.Linq.Expressions;
using System.Threading;

namespace HLab.Ioc;

public static class SingletonLocator<T>
{
    private static bool _singletonInitialized = false;
    private static readonly object _lock = new();

    public static void SetInstance(T instance)
    {
        Locator<T>.SetFactoryExpression(Expression.Constant(instance));
    }
    public static void Set<T1>() where T1 : T, new()
    {
        var singleton = new T1();
        SetInstance(singleton);
        Locator.Enqueue(() =>
        {
            var injectExpression = Locator<T1>.InjectExpression();
            injectExpression.Compile()(singleton);
        });
    }

    public static void SetFactory(Expression<Func<T>> factory) => Locator<T>.SetFactory(() =>
    {
        Monitor.Enter(_lock);
        try
        {
            if (_singletonInitialized) return Locator<T>.Locate();
            var singleton = factory.Compile()();

            var lambda = Expression.Lambda<Func<T>>(Expression.Constant(singleton));

            Locator<T>.SetFactoryExpression(lambda);

            _singletonInitialized = true;
            return singleton;
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    });
}