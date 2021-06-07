using HLab.Core.Annotations;
using HLab.DependencyInjection;
using HLab.DependencyInjection.Annotations;
using Xunit;

namespace HLab.Stashbox.UnitTests.BaseClassMethod
{
    public class A
    { }
    public class B
    { }

    public class BaseClass
    {
        public A A { get; set; }
        [Import]
        public void InjectA(A injected)
        {
            A = injected;
        }
    }

    public class MainClass : BaseClass
    {
        public B B { get; set; }
        [Import]
        public void InjectB(B injected)
        {
            B = injected;
        }
    }

    public class BaseClassMethod
    {
        [Fact]
        public void Test()
        {
            var container = new DependencyInjectionContainer();
            //container.Configure(c => c.ImportMembers(MembersThat.HaveAttribute<ImportAttribute>(),true));
            //container.Register<A>().Register<B>().Register<MainClass>();

            var main = container.Locate<MainClass>();

            Assert.IsType<B>(main.B);
            Assert.IsType<A>(main.A);
        }
    }
}
