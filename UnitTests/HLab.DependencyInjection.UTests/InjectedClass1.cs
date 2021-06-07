using System;
using HLab.DependencyInjection.Annotations;
using Xunit;

namespace HLab.DependencyInjection.UTests
{
    class InjectedClass1
    {
        [Import] private Func<string, InjectedClassWithCtor> _get;
        public InjectedClassWithCtor Get(string s)
        {
            Assert.NotNull(_get);
            return _get(s);
        }
    }
}