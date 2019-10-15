using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace HLab.Notify.PropertyChanged.Benchmark.NF
{
    [ClrJob(baseline: true), /*CoreJob, /*MonoJob, CoreRtJob*/]
    [RPlotExporter, RankColumn]
    public class Bench
    {
        //private DummyBench _n1 = new DummyBench { Property = 42 };
        //private NotifierEmptyBench _n2 = new NotifierEmptyBench { Property = 42 };
        //private NotifierBench _n3 = new NotifierBench { Property = 42 };
        //private DependencyBench _n4 = new DependencyBench { Property = 42 };

        [GlobalSetup]
        public void GlobalSetup()
        {
            new NotifierBench();
        }

        //[Benchmark]
        //public void Dummy()
        //{
        //    _n1.Property++;
        //}

        //[Benchmark]
        //public void Locked()
        //{
        //    _n1.PropertyLocked++;
        //}
        //[Benchmark]
        //public void LockedHelper()
        //{
        //    _n1.PropertyLockedHelper++;
        //}

        //[Benchmark]
        //public void Empty()
        //{
        //    _n2.Property++;
        //}

        //[Benchmark]
        //public void Dependency()
        //{
        //    _n4.Property++;
        //}
        //[Benchmark]
        //public void Real()
        //{
        //    _n3.Property++;
        //}

        [Benchmark(Baseline = true)]
        public void DependencyNew()
        {
            DependencyBench n = new DependencyBench();// { Property = 42 };
            //n.Property++;
        }

        [Benchmark]
        public void RealNew()
        {
            NotifierBench n = new NotifierBench();//{ Property = 42 };
            //n.Property++;
        }

    }
}
