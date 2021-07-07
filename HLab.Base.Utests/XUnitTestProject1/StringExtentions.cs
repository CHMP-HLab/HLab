using HLab.Base.Extensions;
using System;
using System.Linq;
using Xunit;

namespace HLab.Base.UTests
{
    public class StringExtensionsUnitTests
    {
        [Fact]
        public void Test()
        {
            var s = "{42}";
            var r = s.GetInside('{','}');
            Assert.Equal("42",r.First());
        }

        [Fact]
        public void Test2()
        {
            var s = "{42} bla {test}";
            var r = s.GetInside('{','}').ToArray();
            Assert.Equal("42",r[0]);
            Assert.Equal("test",r[1]);
        }

        [Fact]
        public void Test3()
        {
            var s = "{42{test}";
            Assert.Throws<ArgumentException>(()=>s.GetInside('{', '}').ToArray());
        }

        [Fact]
        public void Test4()
        {
            var s = "{42{test}}";
            var r = s.GetInside('{','}').ToArray();
            Assert.Equal("42test",r[0]);
        }
    }

}
