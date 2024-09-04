using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace HLab.Ioc;

public class Locator
{
    private static readonly Queue<Action> _initSingletons = new();

    public static void Enqueue(Action action) => _initSingletons.Enqueue(action);

    public static void InitSingletons()
    {
        while (_initSingletons.TryDequeue(out var action))
        {
            action();
        }
    }


    private static readonly ConcurrentDictionary<Type, Type> _openGenerics = new();
    public static void SetOpenGenericFactory(Type type, Type factoryType)
    {
        if (!factoryType.IsGenericTypeDefinition) throw new Exception("Should use generic");
        if (!type.IsGenericTypeDefinition) throw new Exception("Should use generic");

        _openGenerics.TryAdd(type, factoryType);
    }
    public static Func<T> GetOpenGenericFactory<T>()
    {
        var t = _openGenerics[typeof(T).GetGenericTypeDefinition()];
        var result = t.MakeGenericType(typeof(T).GetGenericArguments()[0]);
        var locator = typeof(Locator<>).MakeGenericType(result);
        var locate = locator.GetMethod("Locate");
        return () => (T)locate.Invoke(null, null);
    }
    public static Expression GetOpenGenericExpression<T>()
    {
        var type = typeof(T).GetGenericTypeDefinition();
        if (_openGenerics.ContainsKey(type))
        {
            var t = _openGenerics[type];
            var result = t.MakeGenericType(typeof(T).GetGenericArguments()[0]);
            var locator = typeof(Locator<>).MakeGenericType(result);
            var locate = locator.GetMethod("LocateExpression");
            return (Expression)locate.Invoke(null, null);
        }
        return null;
        throw new IocLocateException($"Type not found : {type}");
    }

    public static void Configure()
    {
        Locator<Func<Type, object>>.SetFactoryExpression(
            () => t => typeof(Locator<>).MakeGenericType(t).GetMethod("Locate").Invoke(null, null)
        );
    }
    protected static T Locate<T>() => Locator<T>.Locate();

    public static void SetFactory(Type type, Func<object> factory)
    {
        throw new NotImplementedException();
    }

    public static Expression GetExpressionLocator(Type type)
    {
        var locator = typeof(Locator<>).MakeGenericType(type);
        var method = locator.GetMethod("LocateExpression");
        var expression = (Expression)method.Invoke(null, null);
        return expression;
    }

    public static Expression GetExpressionLocator(Type type, ParameterExpression[] parameters)
    {
        var locator = typeof(Locator<>).MakeGenericType(type);
        var targetType = (Type)locator.GetProperty("TargetType").GetValue(null);

        if (targetType != type) locator = typeof(Locator<>).MakeGenericType(targetType);

        var method = locator.GetMethod("AutoFactoryExpression");
        var expression = (Expression)method.Invoke(null, new object[] { parameters });
        return expression;
    }
    public static void Set(Type iface, Type t)
    {
        var locator = typeof(Locator<>).MakeGenericType(iface);
        var mSet = locator.GetMethod("Set",Array.Empty<Type>());
        var mSetG = mSet.MakeGenericMethod(t);

        mSetG.Invoke(null, null);
    }

}

public static class Locator<T>
{
    public static Type TargetType { get; private set; } = typeof(T);
    static Locator()
    {
        var type = typeof(T);

        if (type.IsInterface)
        {
            if (typeof(T).IsGenericType)
            {

                var factory = Locator.GetOpenGenericExpression<T>();
                if (factory != null)
                {
                    SetFactoryExpression(factory);
                }
            }
            return;
        }

        if (type.IsAbstract) return;
        if (type.IsValueType) return;

        if (type.IsAssignableTo(typeof(Delegate)))
        {
            if (type == typeof(Func<Type, object>)) return;

            var def = type.GetGenericTypeDefinition();
            if (def.BaseType == typeof(MulticastDelegate) && def.Name.StartsWith("Func`"))
            {
                var argumentsTypes = type.GetGenericArguments();
                var n = argumentsTypes.Length - 1;
                var returnType = argumentsTypes[n];
                if (n > 0)
                {
                    var parameters = new ParameterExpression[n];
                    for (int i = 0; i < n; i++)
                    {
                        parameters[i] = Expression.Parameter(argumentsTypes[i]);
                    }
                    var body = Locator.GetExpressionLocator(returnType, parameters);

                    var lambda = Expression.Lambda(type, body, parameters);

                    SetFactoryExpression(lambda);
                }
                else
                {
                    var body = Locator.GetExpressionLocator(returnType);
                    var lambda = Expression.Lambda(type, body);
                    SetFactoryExpression(lambda);
                }

            }
            else
            { }

            return;
        };

        try
        {
            SetAutoFactoryExpression();
        }
        catch (Exception)
        { }
    }

    private static Expression _factoryExpression;
    private static Func<T> _factory;
    public static T Locate() => _factory();
    public static Expression LocateExpression() => _factoryExpression;

    public static void Set<T1>() where T1 : T
    {
        TargetType = typeof(T1);
        try
        {
            SetFactoryExpression(Locator<T1>.LocateExpression());
        }
        catch (Exception)
        { }
    }

    //public static void Set<T1>(T1 dummy) where T1 : T
    //{
    //    Set<T1>();
    //}

    public static void SetFactoryExpression(Expression factoryExpression)
    {
        Interlocked.Exchange(ref _factoryExpression, factoryExpression);
        var expression = Expression.Lambda<Func<T>>(factoryExpression);
        var lambda = expression.Compile();
        SetFactory(lambda);
    }
    public static void SetFactoryExpression(Expression<Func<T>> factoryExpression)
    {
        Interlocked.Exchange(ref _factoryExpression, factoryExpression.Body);
        SetFactory(factoryExpression.Compile());
    }

    public static void SetFactory(Func<T> factory)
    {
        Interlocked.Exchange(ref _factory, factory);
    }

    private static bool IsInjectMethod(MemberInfo info) => info.Name == "Inject";

    public static void SetAutoFactoryExpression()
    {
        SetFactoryExpression(AutoFactoryExpression());
    }

    public static Expression AutoFactoryExpression(ParameterExpression[] parameters = null)
    {
        var methodsStack = new Stack<MethodInfo>();
        var type = typeof(T);

        while (type != typeof(object) && type != null)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods.Where(IsInjectMethod))
            {
                methodsStack.Push(method);
            }
            type = type.BaseType;
        }

        var constructors = typeof(T).GetConstructors();

        // TODO : Here we get the first constructor that works, maybe we should use the one that consume most parameters
        foreach (var constructor in constructors)
        {
            try
            {
                var instance = Expression.Variable(typeof(T), "instance");
                List<Expression> list = new()
                {
                    Expression.Assign(instance, FactoryExpression(constructor, parameters))
                };

                while (methodsStack.TryPop(out var method))
                {
                    list.Add(MethodInjectionExpression(instance, method, parameters));
                }
                list.Add(instance);
                var expression = Expression.Block(new[] { instance }, list);
                return expression;
            }
            catch (IocLocateException)
            { }

        }
        throw new Exception($"No constructor found on class {typeof(T)}");
        // Injection have to be done from base class to top level to ensure base class to be populated first
    }

    public static Expression<Action<T>> InjectExpression(ParameterExpression[] parameters = null)
    {
        var methodsStack = new Stack<MethodInfo>();
        var type = typeof(T);

        while (type != typeof(object) && type != null)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods.Where(IsInjectMethod))
            {
                methodsStack.Push(method);
            }
            type = type.BaseType;
        }
        var list = new List<Expression>();
        var instance = Expression.Parameter(typeof(T));
        while (methodsStack.TryPop(out var method))
        {
            list.Add(MethodInjectionExpression(instance, method, parameters));
        }

        return Expression.Lambda<Action<T>>(Expression.Block(list), new ParameterExpression[] { instance });
    }

    private static Expression[] ArgumentsExpression(MethodBase info, List<ParameterExpression> availableParameters)
    {
        var parameters = info.GetParameters();
        var size = parameters.Length;
        if (size == 0) return Array.Empty<ParameterExpression>();

        var argumentExpression = new Expression[size];

        try
        {
            for (int i = 0; i < size; i++)
            {
                argumentExpression[i] = ArgumentExpression(parameters[i], availableParameters);
            }
            return argumentExpression;

        }
        catch (IocLocateException e)
        {
            throw new IocLocateException($"{e.Message} in {info.DeclaringType}.{info.Name}", e);
        }
    }

    private static Expression ArgumentExpression(ParameterInfo info, List<ParameterExpression> availableParameters)
    {
        var available = availableParameters?.FirstOrDefault(e => e.Type.IsAssignableTo(info.ParameterType));
        if (available != null)
        {
            availableParameters.Remove(available);
            return available;
        }
            
        return ArgumentExpression(info);
    }

    private static Expression ArgumentExpression(ParameterInfo info)
    {
        var locator = Locator.GetExpressionLocator(info.ParameterType);
        if (locator != null) return locator;

        if (info.IsOptional) return Expression.Constant(info.DefaultValue);

        throw new IocLocateException($"Unable to locate {info.ParameterType.Name}");
    }

    private static Expression FactoryExpression(ConstructorInfo info, ParameterExpression[] parameters)
    {
        var arguments = ArgumentsExpression(info, parameters?.ToList());
        return Expression.New(info, arguments);
    }

    private static Expression MethodInjectionExpression(Expression instance, MethodInfo info, ParameterExpression[] parameters)
    {
        var arguments = ArgumentsExpression(info, parameters?.ToList());
        return Expression.Call(instance, info, arguments);
    }

}