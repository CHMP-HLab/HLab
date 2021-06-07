using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace HashBenchmark
{
    [/*ClrJob(baseline: true),*/ CoreJob, /*MonoJob, CoreRtJob*/]
    [RPlotExporter, RankColumn]
    public class BenchCast
    {
        private Object _obj = new BasicClass("test");

        [GlobalSetup]
        public void GlobalSetup()
        {
        }

        [Benchmark]
        public BasicClass Is()
        {
            if (_obj is BasicClass b) return b;
            return null;
        }

        [Benchmark]
        public BasicClass As()
        {
            return (_obj as BasicClass);
        }

        [Benchmark(Baseline = true)]
        public BasicClass Cast()
        {
            return (BasicClass) _obj;
        }

    }
}
