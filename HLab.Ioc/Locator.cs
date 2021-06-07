using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;

namespace HLab.Ioc
{
    public class Locator
    {
        private static Queue<Action> initSingletons = new();

        public static void Enqueue(Action action) => initSingletons.Enqueue(action);

        public static void InitSingletons()
        {
            while(initSingletons.TryDequeue(out var action))
            {
                action();
            }
        }


        private static ConcurrentDictionary<Type,Type> _openGenerics = new();
        public static void SetOpenGenericFactory(Type type, Type factoryType)
        {
            if(!factoryType.IsGenericTypeDefinition) throw new Exception("Should use generic");
            if(!type.IsGenericTypeDefinition) throw new Exception("Should use generic");

            _openGenerics.TryAdd(type,factoryType);
        }
        public static Func<T> GetOpenGenericFactory<T>()
        {
            var t = _openGenerics[typeof(T).GetGenericTypeDefinition()];
            var result = t.MakeGenericType(typeof(T).GetGenericArguments()[0]);
            var locator = typeof(Locator<>).MakeGenericType(result);
            var locate = locator.GetMethod("Locate");
            return () => (T)locate.Invoke(null,null);
        }
        public static Expression GetOpenGenericExpression<T>()
        {
            var type = typeof(T).GetGenericTypeDefinition();
            if(_openGenerics.ContainsKey(type))
            {
                var t = _openGenerics[type];
                var result = t.MakeGenericType(typeof(T).GetGenericArguments()[0]);
                var locator = typeof(Locator<>).MakeGenericType(result);
                var locate = locator.GetMethod("LocateExpression");
                return (Expression)locate.Invoke(null,null);
            }
            return null;
            throw new Exception($"Type not found : {type}");
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
            var expression =  (Expression)method.Invoke(null,null);
            return expression;
        }

        public static Expression GetExpressionLocator(Type type, ParameterExpression[] parameters)
        {
            var locator = typeof(Locator<>).MakeGenericType(type);
            var targetType = (Type)locator.GetProperty("TargetType").GetValue(null);

            if(targetType!=type) locator = typeof(Locator<>).MakeGenericType(targetType);

            var method = locator.GetMethod("AutoFactoryExpression");
            var expression =  (Expression)method.Invoke(null,new object[]{parameters });
            return expression;
        }
        public static void Set(Type iface, Type t)
        {
            var locator = typeof(Locator<>).MakeGenericType(iface);
            var mSet = locator.GetMethod("Set");
            var mSetG = mSet.MakeGenericMethod(t);

            mSetG.Invoke(null,null);
        }

    }

    public static class Locator<T>
    {
        public static Type TargetType {get ; private set;} = typeof(T);
        static Locator()
        {
            var type = typeof(T);

            if(type.IsInterface)
            {
                if(typeof(T).IsGenericType) {

                    var factory = Locator.GetOpenGenericExpression<T>();
                    if (factory!=null)
                    {
                        SetFactoryExpression(factory);
                    }
                }
                return;
            }

            if(type.IsAbstract) return;
            if(type.IsValueType) return;

            if(type.IsAssignableTo(typeof(Delegate))) 
            {
                if(type == typeof(Func<Type,object>)) return;

                var def = type.GetGenericTypeDefinition();
                if(def.BaseType==typeof(MulticastDelegate) && def.Name.StartsWith("Func`"))
                {
                    var argumentsTypes = type.GetGenericArguments();
                    var n = argumentsTypes.Length - 1;
                    var returnType = argumentsTypes[n];
                    if(n>0)
                    {
                        var parameters = new ParameterExpression[n];
                        for(int i = 0; i<n; i++)
                        {
                            parameters[i] = Expression.Parameter(argumentsTypes[i]);
                        }
                        var body = Locator.GetExpressionLocator(returnType,parameters);

                        var lambda = Expression.Lambda(type,body,parameters);

                        SetFactoryExpression(lambda);
                    }
                    else
                    {
                        var body = Locator.GetExpressionLocator(returnType);
                        var lambda = Expression.Lambda(type,body);
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
            catch(Exception)
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

        private static bool IsInjectMethod(MethodInfo info)
        {
            if(info.Name=="Inject") return true;
            return false;
        }


        public static void SetAutoFactoryExpression()
        {
            SetFactoryExpression(AutoFactoryExpression());
        }

        public static Expression AutoFactoryExpression(ParameterExpression[] parameters = null)
        {
            var methodsStack = new Stack<MethodInfo>();
            var type = typeof(T);

            while(type!=typeof(object) && type!=null)
            {
                var methods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
                foreach(var method in methods)
                {
                    if(IsInjectMethod(method)) methodsStack.Push(method);
                }
                type = type.BaseType;
            }

            var constructors = typeof(T).GetConstructors();

            foreach(var constructor in constructors)
            {
                var instance = Expression.Variable(typeof(T),"instance");
                var list = new List<Expression>();
                list.Add(Expression.Assign(instance, FactoryExpression(constructor,parameters)));
                while(methodsStack.TryPop(out var method))
                {
                    list.Add(MethodInjectionExpression(instance,method,parameters));
                }
                list.Add(instance);
                var expression = Expression.Block(new ParameterExpression[] { instance },list);
                return expression;
            }
            throw new Exception($"No constructor found on class {typeof(T)}");
            // Injection have to be done from base class to top level to ensure base class to be populated first
        }

        public static Expression<Action<T>> InjectExpression(ParameterExpression[] parameters = null)
        {
            var methodsStack = new Stack<MethodInfo>();
            var type = typeof(T);

            while(type!=typeof(object) && type!=null)
            {
                var methods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
                foreach(var method in methods)
                {
                    if(IsInjectMethod(method)) methodsStack.Push(method);
                }
                type = type.BaseType;
            }
            var list = new List<Expression>();
            var instance = Expression.Parameter(typeof(T));
            while(methodsStack.TryPop(out var method))
            {
                list.Add(MethodInjectionExpression(instance,method,parameters));
            }
            
            return Expression.Lambda<Action<T>>(Expression.Block(list),new ParameterExpression[] {instance});
        }

        private static Expression[] ArgumentsExpression(MethodBase info, ParameterExpression[] availableParameters)
        {
            var parameters = info.GetParameters();
            var size = parameters.Length;
            if(size==0) return Array.Empty<ParameterExpression>();

            var argumentExpression = new Expression[size];

            for(int i = 0; i<size; i++)
            {
                var type = parameters[i].ParameterType;

                var available = availableParameters?.FirstOrDefault(e => e.Type.IsAssignableTo(type));

                if(available!=null) argumentExpression[i] = available;
                else
                {
                    var locator = Locator.GetExpressionLocator(type);

                    if(locator==null)
                    {
                        if(parameters[i].IsOptional) locator = Expression.Constant(parameters[i].DefaultValue);
                        else throw new Exception($"Unable to locate {type.Name} for {typeof(T)}.{info.Name}]"); 
                    }
                    argumentExpression[i] = locator;
                }
            }
            return argumentExpression;
        }

        private static Expression FactoryExpression(ConstructorInfo info, ParameterExpression[] parameters)
        {
            var arguments = ArgumentsExpression(info, parameters);
            return Expression.New(info,arguments);
        }

        private static Expression MethodInjectionExpression(Expression instance, MethodInfo info, ParameterExpression[] parameters)
        {
            var arguments = ArgumentsExpression(info, parameters);
            return Expression.Call(instance,info,arguments);
        }

    }

    public static class SingletonLocator<T>
    {
        private static bool _singletonInitialized = false;
        private static object _lock = new object();

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

        public static void SetFactory(Expression<Func<T>> factory) => Locator<T>.SetFactory( () =>
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

    public static class EnumerableLocator<T>
    {
        static List<Expression> _expressions = new();
        static ParameterExpression list = Expression.Variable(typeof(List<T>));
        static MethodInfo Addmethod = typeof(List<T>).GetMethod("Add",new Type[]{typeof(T)});
        static ConstructorInfo listConstructor = typeof(List<T>).GetConstructor(Array.Empty<Type>());
        public static void Add<T1>() where T1 : T
        {
            var expression = Locator<T1>.LocateExpression();
            _expressions.Add(Expression.Call(list, Addmethod, expression));

            var e = new List<Expression>();
            e.Add(Expression.Assign(list,Expression.New(listConstructor)));
            e.AddRange(_expressions);
            e.Add(list);

            var block = Expression.Block(typeof(IEnumerable<T>), new ParameterExpression[]{ list}, e);

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

        private static MethodInfo _addMethod = typeof(EnumerableLocator<T>).GetMethod("Add",1,new Type[]{ });
        public static void Add(Type t)
        {
            if(t.IsGenericType) return;

            var method = _addMethod.MakeGenericMethod(t);
            method.Invoke(null,null);
        }

        private static MethodInfo _addAutoFactoryMethod = typeof(EnumerableLocator<T>).GetMethod("AddAutoFactory",1,new Type[]{ });
        public static void AddAutoFactory(Type t)
        {
            if(t.IsGenericType) return;

            var method = _addAutoFactoryMethod.MakeGenericMethod(t);
            method.Invoke(null,null);
        }
    }


}
