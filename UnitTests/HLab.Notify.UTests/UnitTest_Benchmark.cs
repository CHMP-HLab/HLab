using HLab.DependencyInjection.Annotations;
using System;
using System.Diagnostics;
using HLab.Base;
using HLab.DependencyInjection;
using HLab.Notify.Annotations;
using Xunit;

namespace HLab.Notify.UTests
{


    public class UnitTest_Benchmark
    {
        private static readonly IExportLocatorScope Container = new DependencyInjectionContainer()
            //            .Configure(e => e.Export<NotifierService>().As<INotifierService>().Singleton())
            .Configure(e => e
                .Export(typeof(Notifier<>)).As<INotifier>().GenericAsTarget()
                .Export<EventHandlerService>().As<IEventHandlerService>()
                .Export(typeof(NotifierClass<>)).As(typeof(INotifierClass<>)).Singleton()
            );

        private long Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed.Ticks;
        }

        class RefClass {
            private int _value;

            public int Value
            {
                get => _value;
                set => _value = value;
            }
        }


        [Fact]
        public void BenchmarkPropertySet()
        {
            var rf = new RefClass();
            var a = Container.Locate<TestNotifierLevel0>();
            var j = 2000000;

            var r0 = rf.Value-1;
            var r1 = a.Value-1;
            var r = -1;

            void Ref0a()
            {
                r0 = rf.Value;
            }
            void Ref0b()
            {
                r0 = a.Value;
            }
            ;
            void Ref1()
            {
                for (int i = 0; i < j; i++) rf.Value += i;
                r1 = rf.Value;
            }
            void Test()
            {
                for (int i = 0; i < j; i++) a.Value += i;
                r = a.Value;
            }

            var t0a = (double)Time(Ref0a);
            var t0b = (double)Time(Ref0b);
            var t1 = ((double)Time(Ref1)-t0a)/j;
            var t = ((double)Time(Test)-t0b)/j;

            Assert.True(t<=50, t0a.ToString() + " / " + t0b.ToString() + " > " + t1.ToString()+ " -> " + t.ToString() );
        }
    }
}
