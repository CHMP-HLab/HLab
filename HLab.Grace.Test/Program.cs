using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using System;
using System.Reflection;
using Xunit;

namespace Grace.Test
{
    
    public class Exported
    { }
    
    public interface ITest
    { }

    [Export(typeof(ITest))]
    public class Test : ITest
    {
        int i = 1;

        private  Exported _exported;

        [Import]
        public void Inject(Exported exported)
        {
            _exported = exported;

            Assert.Equal(1,i);
            Console.WriteLine($"Injected {i++}");
        }

        public Test()
        {
            Console.WriteLine($"Constructor");
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

            var c = container.Locate<ITest>();

            Console.ReadKey();
        }
    }
}
