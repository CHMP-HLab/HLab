using System;
using HLab.DependencyInjection.Annotations;
using Xunit;

namespace HLab.DependencyInjection.UTests
{
    public class UnitTest1
    {
        private static readonly IExportLocatorScope Container = new DependencyInjectionContainer();

        [Fact]
        public void TestImportScope()
        {
            var scope = Container.Locate<IExportLocatorScope>();
            Assert.Same(Container,scope);
        }

        [Fact]
        public void TestSingleton()
        {
            Container.Configure(c => c.Export<Singleton>().As<ISingleton>().Singleton());

            var s1 = Container.Locate<ISingleton>();
            var s2 = Container.Locate<ISingleton>();

            Assert.Same(s1,s2);
        }


        [Fact]
        public void TestSimpleInjection()
        {
            var ic1 = Container.Locate<InjectedClass1>();
            var ic0 = ic1.Get("bob");

            Assert.Equal("bob", ic0.Value);
        }

        [Fact]
        public void TestRecursiveSingleton()
        {
            Container.Configure(c => c.Export<Singleton>().As<ISingleton>().Singleton());

            var r = Container.Locate<RecursiveInjectedSingletonParent>();

            Assert.Same(r.Singleton,r.Child.Singleton);
        }

        [Fact]
        public void TestInjectionWithMultipleInterfaces()
        {
            Container.Configure(c => c.Export<InjectedClassDualInterfaces>().As<IInjectedAsA>());
            Container.Configure(c => c.Export<InjectedClassDualInterfaces>().As<IInjectedAsB>());

            var r1 = Container.Locate<IInjectedAsA>();
            var r2 = Container.Locate<IInjectedAsB>();

            Assert.IsType<InjectedClassDualInterfaces>(r1);
            Assert.IsType<InjectedClassDualInterfaces>(r2);
            Assert.NotSame(r1,r2);
        }
        [Fact]
        public void TestInjectionWithMultipleInterfacesSingleton()
        {
            Container.Configure(c => c.Export<InjectedClassDualInterfaces>().As<IInjectedAsA>().Singleton());
            Container.Configure(c => c.Export<InjectedClassDualInterfaces>().As<IInjectedAsB>().Singleton());

            var r1 = Container.Locate<IInjectedAsA>();
            var r2 = Container.Locate<IInjectedAsB>();

            Assert.IsType<InjectedClassDualInterfaces>(r1);
            Assert.IsType<InjectedClassDualInterfaces>(r2);
            Assert.Same(r1, r2);
        }

        [Fact]
        public void TestInjectionWithMultipleInterfacesAnnotated()
        {
            Container.Configure(c => c.Export<InjectedClassDualInterfaces>().AsAnnotated());

            var r1 = Container.Locate<IInjectedAsA>();
            var r2 = Container.Locate<IInjectedAsB>();

            Assert.IsType<InjectedClassDualInterfaces>(r1);
            Assert.IsType<InjectedClassDualInterfaces>(r2);
            Assert.NotSame(r1, r2);
        }
    }
}
