using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BenchmarkDotNet.Attributes;

namespace HashBenchmark
{
    public class TestNewClass
    { }

    [/*ClrJob(baseline: true),*/ CoreJob, /*MonoJob, CoreRtJob*/]
    [RPlotExporter, RankColumn]
    public class BenchNew
    {


        [Benchmark(Baseline = true)]
        public BasicClass WithNew()
        {
            return new BasicClass("This is a Test");
        }

        ConstructorInfo ctor = typeof(BasicClass).GetConstructor(new Type[] { typeof(string) });

        [Benchmark]
        public BasicClass WithGetSafeUninitializedObject()
        {
            var o = (BasicClass)FormatterServices
                .GetSafeUninitializedObject(typeof(BasicClass));
            ctor.Invoke(o,new object[]{ "This is a Test" });
            return o;
        }

        [Benchmark]
        public BasicClass WithGetUninitializedObject()
        {
            var o = (BasicClass)FormatterServices
                .GetUninitializedObject(typeof(BasicClass));
            ctor.Invoke(o, new object[] { "This is a Test" });
            return o;
        }

        [Benchmark]
        public BasicClass WithActivator()
        {
            return (BasicClass)Activator.CreateInstance(typeof(BasicClass), "This is a Test" );
        }

        //[Benchmark]
        //public object WithGetUninitializedObject()
        //{
        //    return FormatterServices.GetUninitializedObject(typeof(object));
        //}

        //[Benchmark]
        //public object WithGetSafeUninitializedObject()
        //{
        //    return FormatterServices.GetSafeUninitializedObject(typeof(object));
        //}

        //private readonly Func<object>  GetInstance = 
        //    Expression.Lambda<Func<object>>(Expression.New(typeof(object))).Compile();  

        //[Benchmark]
        //public object WithExpression()
        //{
        //    return GetInstance();
        //}
    }
}