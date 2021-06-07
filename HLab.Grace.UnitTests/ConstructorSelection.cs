using HLab.DependencyInjection;
using System;

using Xunit;

namespace HLab.Stashbox.UnitTests
{
    class ClassA
    { }

    class ClassB
    {
    }

    class InjectedClass
    {
        public ClassA ClassA;
        public ClassB ClassB;

        public InjectedClass(ClassA classA)
        {
            ClassA = classA;
        }

        public InjectedClass(ClassB classB)
        {
            ClassB = classB;
        }
    }

    public class ConstructorSelection
    {
        [Fact]
        public void Test()
        {
            var container = new DependencyInjectionContainer();
            container.ExportAssembly(typeof(ClassA).Assembly);

            var funcA = container.Locate<Func<ClassA, InjectedClass>>();
            var classA = container.Locate<ClassA>();
            var injectedA = funcA(classA);

            var funcB = container.Locate<Func<ClassB, InjectedClass>>();
            var classB = container.Locate<ClassB>();
            var injectedB = funcB(classB);

            Assert.Equal(classA,injectedA.ClassA);
            Assert.Equal(classB,injectedB.ClassB); //first constructor used, with newly created ClassA and classB not used
        }
    }
}
