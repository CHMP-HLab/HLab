using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace HashBenchmark
{
    [/*ClrJob(baseline: true),*/ CoreJob, /*MonoJob, CoreRtJob*/]
    [RPlotExporter, RankColumn]
    public class BenchMethod
    {
        //private object[] data;

        //[Params(10)]
        //public int N;

        //[GlobalSetup]
        //public void Setup()
        //{
        //    data = new object[N];
        //    for(int i=0; i<N; i++) data[i] = new object();
        //}

        delegate int BenchDelegate(int value);

        BenchDelegate benchDelegate = (i) => i + i;

        private Func<int, int> benchFunc = i => i + i;

        private Expression<Func<int, int>> benchExpr = i => i + i;


        //public Func<int, int> benchExprCompiled = Expression.Lambda<Func<int, int>>(i=>i+i).Compile();
        public int benchMethod(int i) => i + i;
        public int benchMethod2(int i) => benchMethod(i);
        public int benchMethod3(int i) => benchMethod2(i);
        public int benchMethod4(int i) => benchMethod3(i);
        public int benchMethod5(int i) => benchMethod4(i);

        private int b = 42;

        [Benchmark(Baseline = true)]
        public int WithNone()
        {
            return b = benchMethod(b);
        }

        [Benchmark]
        public int WithDelegate()
        {
            return b = benchDelegate(b);
        }

        [Benchmark]
        public int WithFunc()
        {
            return b = benchFunc(b);
        }
        [Benchmark]
        public int WithCascaded()
        {
            return b = benchMethod5(b);
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