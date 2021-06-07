using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace HLab.Ioc.Benchmark_
{

    public class Test {
        
        public Func<object> Factory;
        public object Locate() => Factory();
    }

    public class SingletonTest
    { }


    public class GenericVsDict
    {
        public ConcurrentDictionary<Type,Test> ConcurrentDictionary = new();
        public Dictionary<Type,Test> Dictionary = new();
        public Type Type = typeof(object);

        public GenericVsDict()
        {
            Dictionary.Add(Type,new Test{ Factory = ()=>new object() });
            ConcurrentDictionary.TryAdd(Type,new Test{ Factory = ()=>new object() });
            Locator<object>.SetFactory(()=>new object());
            SingletonLocator<SingletonTest>.SetFactory(()=> new SingletonTest());

            Locator<SingletonTest>.Locate();
        }

        [Benchmark]
        public object GenericTest()
        {
            return Locator<object>.Locate();
        }

        [Benchmark]
        public object DictionaryTest()
        {
            return Dictionary[Type].Locate();
        }

        [Benchmark]
        public object ConcurrentDictionaryTest()
        {
            return ConcurrentDictionary[Type].Locate();
        }
        [Benchmark]
        public object SingletonTest()
        {
            return Locator<SingletonTest>.Locate();
        }
    }


    public class HomeSingleton
    {
        private object _lock = new object();
        private bool _initialized = false;

        public HomeSingleton()
        {
            SetInitFactory(() => new object());
        }

        private Func<object> _factory;
        public void SetFactory(Func<object> factory)
        {
            Interlocked.Exchange(ref _factory, factory);
        }
        public void SetInitFactory(Func<object> factory) => SetFactory( () =>
        {
            Monitor.Enter(_lock);
            try
            {
                if (_initialized) return Locate();
                var singleton = factory();
                SetFactory(() => singleton);
                _initialized = true;
                return singleton;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        });
        public object Locate() => _factory();
    }
    public class LazySingleton
    {
        Lazy<Func<object>> Lazy = new(()=> new object());

        public object Locate() => Lazy.Value();

    }

    public class HomeMadeVsLasy
    {
        HomeSingleton HomeSingleton = new();
        LazySingleton LazySingleton = new();

        public HomeMadeVsLasy()
        {
            HomeSingleton.Locate();LazySingleton.Locate();
        }


        [Benchmark]
        public object Home()
        {
            return HomeSingleton.Locate();
        }

        [Benchmark]
        public object Lazy()
        {
            return LazySingleton.Locate();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<GenericVsDict>();
            BenchmarkRunner.Run<HomeMadeVsLasy>();
        }
    }
}
