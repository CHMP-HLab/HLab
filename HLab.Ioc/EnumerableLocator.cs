using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HLab.Ioc;

public static class EnumerableLocator<T>
{
    static List<Expression> _expressions = new();
    static ParameterExpression list = Expression.Variable(typeof(List<T>));
    static MethodInfo Addmethod = typeof(List<T>).GetMethod("Add", new Type[] { typeof(T) });
    static ConstructorInfo listConstructor = typeof(List<T>).GetConstructor(Array.Empty<Type>());
    public static void Add<T1>() where T1 : T
    {
        var expression = Locator<T1>.LocateExpression();
        _expressions.Add(Expression.Call(list, Addmethod, expression));

        var e = new List<Expression>();
        e.Add(Expression.Assign(list, Expression.New(listConstructor)));
        e.AddRange(_expressions);
        e.Add(list);

        var block = Expression.Block(typeof(IEnumerable<T>), new ParameterExpression[] { list }, e);

        Locator<IEnumerable<T>>.SetFactoryExpression(block);
    }

    public static void Add<T1>(Expression<Func<T1>> factory) where T1 : T
    {
        Locator<T1>.SetFactoryExpression(factory);
        Add<T1>();
    }

    public static void AddAutoFactory<T1>() where T1 : T
    {
        Locator<T1>.SetAutoFactoryExpression();
        Add<T1>();
    }

    private static MethodInfo _addMethod = typeof(EnumerableLocator<T>).GetMethod("Add", 1, new Type[] { });
    public static void Add(Type t)
    {
        if (t.IsGenericType) return;

        var method = _addMethod.MakeGenericMethod(t);
        method.Invoke(null, null);
    }

    private static MethodInfo _addAutoFactoryMethod = typeof(EnumerableLocator<T>).GetMethod("AddAutoFactory", 1, new Type[] { });
    public static void AddAutoFactory(Type t)
    {
        if (t.IsGenericType) return;

        var method = _addAutoFactoryMethod.MakeGenericMethod(t);
        method.Invoke(null, null);
    }
}