using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using HLab.Base;
using HLab.DependencyInjection;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;
using Xunit;

namespace HLab.Notify.UTests
{
    public partial class UnitTestNotifier
    {
        private static readonly IExportLocatorScope Container = new DependencyInjectionContainer()
//            .Configure(e => e.Export<NotifierService>().As<INotifierService>().Singleton())
            .Configure(e => e
                    .Export(typeof(Notifier<>)).As<INotifier>().GenericAsTarget()
                    .Export<EventHandlerService>().As<IEventHandlerService>()
                    .Export(typeof(NotifierClass<>)).As(typeof(INotifierClass<>)).Singleton()

                    .Export<NotifierPropertyActivator>()
                    .As<IActivator>()
                    .WhenInjectedInto<NotifierObject>()
                    .When(t => t.Context.TargetMemberInfo is PropertyInfo)
                    .WithPriority(1)
                    .Singleton()
                );
            
        [Fact]
        public void GetNotifierClassTest()
        {

//            var service = Container.Locate<INotifierService>();

            var c1 = Container.Locate<INotifierClass<TestNotifierLevel0>>();
            var c2 = Container.Locate < INotifierClass<TestNotifierLevel1>>();
            var c3 = Container.Locate < INotifierClass<TestNotifierLevel0>>(); 

            Assert.Same(c1,c3);
            Assert.NotSame(c1,c2);

            Assert.Equal(typeof(TestNotifierLevel0),c1.ClassType);
            Assert.Equal(typeof(TestNotifierLevel1),c2.ClassType);
        }
        [Fact]
        public void ValueAffectation()
        {
            var t = Container.Locate<TestClass>();
            t.Result = 42;

            Assert.Equal(42, t.Result);
        }

        [Fact]
        public void ObjectInNotifierChainTest()
        {
            var t = Container.Locate<TestClass>();
            t.TestObjectLevel1.TestNotifierLevel0.Value = 42;

            Assert.Equal(42,t.Result);
        }

        [Fact]
        public void ObjectInNotifierChainTest2()
        {
            var t = Container.Locate<TestClassConst>();
            t.TestObjectLevel1.TestNotifierLevel0.Value = 42;

            Assert.Equal(42, t.Result);
        }

        [Fact]
        public void TestDefaultGetter()
        {
            var n = Container.Locate<TestGetter>();
            var ValueChanges = 0;
            var SourceChanges = 0;
            var OtherChanges = 0;

            n.PropertyChanged += (s,e) =>
            {
                switch (e.PropertyName)
                {
                    case "Value": ValueChanges++;
                        break;
                    case "Source": SourceChanges++;
                        break;
                    default: OtherChanges++;
                        break;
                }
              
            };

            n.Source = 42;

            Assert.Equal(42,n.Value);
            Assert.Equal(1, SourceChanges);
            Assert.Equal(1, ValueChanges);
            Assert.Equal(0, OtherChanges);
        }


        [Fact]
        public void NotifierTestA()
        {
            var c = Container.Locate<SumCollection>();
            var b = c.LinkedCollectionTest;
            var a1 = Container.Locate<TestNotifierLevel0>();
            var a2 = Container.Locate<TestNotifierLevel0>();

            Debug.Assert(b.LinkedCollection.Count == 0);

            b.CollectionTest.Collection.Add(a1);
            Debug.Assert(b.LinkedCollection.Count == 1);

            b.CollectionTest.Collection.Add(a2);
            Debug.Assert(b.LinkedCollection.Count == 2);

            a1.Value = 10;
            Assert.Equal(10, b.Value);

            a2.Value = 5;
            Assert.Equal(15, b.Value);

            Assert.Equal(10, b.CollectionTest.Collection[0].Value);
            Assert.Equal(10, c.LinkedCollectionTest.LinkedCollection[0].Value);

            Assert.Equal(5, b.CollectionTest.Collection[1].Value);
            Assert.Equal(5, c.LinkedCollectionTest.LinkedCollection[1].Value);

            Assert.Equal(15, c.Value);

            b.CollectionTest.Collection.Remove(a2);
            Assert.Equal(10, b.Value);

        }
    }
}
