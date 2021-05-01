using System;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Xunit;

namespace HLab.Grace.UnitTests
{
    public class Exported
    { }

    public interface ITest
    {
        int P { get; }
        int I { get; }
        int C { get; }
    }

    [Export(typeof(ITest))]
    public class Test : ITest
    {
        public int P { get; set; } = 0;
        private Exported _exported;

        [Import]
        public Exported InjectProperty
        {
            get => _exported;

            set
            {
                _exported = value;
                P++;
            }
        }

        public int I { get; set; } = 0;

        [Import]
        public void InjectMethod(Exported exported)
        {
            I++;
        }

        public int C { get; set; } = 0;
        public Test()
        {
            C++;
        }
    }

    public class GraceTest
    {
        [Fact]
        public void Test()
        {
            var container = new DependencyInjectionContainer();

            var assembly = Assembly.GetAssembly(typeof(Test));

            container.Configure(c => c
                .ExportAssembly(assembly).ExportAttributedTypes()
            );

            var test = container.Locate<ITest>();

            Assert.Equal(1,test.C);
            Assert.Equal(1,test.P);
            Assert.Equal(1,test.I);
        }
        
        [Fact]
        public void Test2()
        {
            var container = new DependencyInjectionContainer();

            var assembly = Assembly.GetAssembly(typeof(Test));

            container.Configure(c => c
                .ExportAssembly(assembly).ByInterfaces().ExportAttributedTypes()
            );

            var test = container.Locate<ITest>();

            Assert.Equal(1,test.C);
            Assert.Equal(1,test.P);
            Assert.Equal(1,test.I);
        }
    }
}
