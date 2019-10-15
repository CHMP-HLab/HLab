using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using HLab.Base;
using HLab.DependencyInjection;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Observables;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using Xunit;

namespace HLab.Mvvm.UTests
{
    public class UTestObservableFilter
    {
        private static readonly IExportLocatorScope Container = new DependencyInjectionContainer()
            //            .Configure(e => e.Export<NotifierService>().As<INotifierService>().Singleton())
            .Configure(e => e
           //     .Export(typeof(Notifier<>)).As<INotifier>().GenericAsTarget()
                .Export<EventHandlerService>().As<IEventHandlerService>()
           //     .Export(typeof(NotifierClass<>)).As(typeof(INotifierClass<>)).Singleton()
            );

        [Fact]
        public void Test1()
        {
            var c = Container.Locate<ObservableCollection<object>>();

            var f = Container.Locate<ObservableFilter<object>>().Link(()=>c);

            var o = new object();

            c.Add(o);

            Assert.Contains(o, c);
            Assert.Contains(o, f);

            c.Remove(o);

            Assert.DoesNotContain(o, c);
            Assert.DoesNotContain(o, f);
        }


        class TestClass
        {
            public bool Value { get; }

            public TestClass(bool value)
            {
                Value = value;
            }
        }

        [Fact]
        public void Test2()
        {
            var c = Container.Locate<ObservableCollection<TestClass>>();

            var f = Container.Locate<ObservableFilter<TestClass>>()
                .AddFilter(tc => tc.Value == true)
                .Link(()=>c);

            var v1 = new TestClass(true);
            var v2 = new TestClass(false);

            c.Add(v1);

            Assert.Contains(v1, c);
            Assert.Contains(v1, f);

            c.Add(v2);

            Assert.Contains(v2, c);
            Assert.DoesNotContain(v2, f);
        }

        private class InjectionTest : NotifierObjectLegacy<InjectionTest>
        {
            [Import, TriggerOn(nameof(Source))]
            public ObservableFilter<TestClass> Children
            {
                get => N.Get<ObservableFilter<TestClass>>();
                private set => N.Set(value
                    .AddFilter(e => e.Value)
                    .Link(() => Source)
                );
            }

            [Import] public ObservableCollection<TestClass> Source
            {
                get => N.Get<ObservableCollection<TestClass>>();
                private set => N.Set(value);
            }

            [TriggerOn(nameof(Children), "Item")]
            public bool ChangedState
            {
                get => N.Get(() => Children.Count > 0);
            }

        }

        [Fact]
        public void Test3()
        {
            var test = Container.Locate<InjectionTest>();

            var v1 = new TestClass(true);
            var v2 = new TestClass(false);

            var trigged = false;

            test.PropertyChanged += (c, a) =>
            {
                if (a.PropertyName == "ChangedState") trigged = true;
            };
            

            Assert.False(test.ChangedState);
            Assert.False(trigged);

            test.Source.Add(v1);

            Assert.Contains(v1, test.Source);
            Assert.Contains(v1, test.Children);

            test.Source.Add(v2);

            Assert.Contains(v2, test.Source);
            Assert.DoesNotContain(v2, test.Children);

            Assert.True(test.ChangedState);
            Assert.True(trigged);
        }
    }
}
