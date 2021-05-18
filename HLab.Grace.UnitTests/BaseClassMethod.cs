using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HLab.Grace.UnitTests.BaseClassMethod
{
    public class InjectedClassA
    { }
    public class InjectedClassB
    { }

    public abstract class BaseClass
    {
        public object InjectedBase { get; set; }
        [Import]
        public void Inject(object injected)
        {
            InjectedBase = injected;
        }
    }

    public class MainClass : BaseClass
    {
        public InjectedClassB InjectedMain { get; set; }
        [Import]
        public void Inject(InjectedClassB injected)
        {
            InjectedMain = injected;
        }
    }

    public class BaseClassMethod
    {
        [Fact]
        public void Test()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(c => c.ImportMembers(MembersThat.HaveAttribute<ImportAttribute>(),true));

            var main = container.Locate<MainClass>();

            Assert.NotNull(main.InjectedMain);
            Assert.NotNull(main.InjectedBase);
        }
    }
}
