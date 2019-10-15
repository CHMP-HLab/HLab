using Xunit;

namespace HLab.Base.UTests
{
    public class BasicClass
    {
        public BasicClass(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public int Value { get; set; }

        public override string ToString() => Name;
    }

    public class WeightedLinkedListUnitTest
    {
        [Fact]
        public void Test1()
        {
            var list = new WeightedList<BasicClass>();

            list.Add(new BasicClass("a"));
            list.Add(new BasicClass("b"));
            list.Add(new BasicClass("c"));

            Assert.Equal(0, list.IndexOf(c => c.Name == "a"));
            Assert.Equal(1, list.IndexOf(c => c.Name == "b"));
            Assert.Equal(2, list.IndexOf(c => c.Name == "c"));

            var n = list.Get(c => c.Name == "a");
            list.CheckConsistency();
            Assert.Equal("a", n.Name);

            n = list.Get(c => c.Name == "c");
            list.CheckConsistency();
            Assert.Equal(1, list.IndexOf(n));

            n = list.Get(c => c.Name == "c");
            list.CheckConsistency();
            Assert.Equal(0, list.IndexOf(n));

            n = list.Get(c => c.Name == "c");
            list.CheckConsistency();
            n = list.Get(c => c.Name == "c");
            list.CheckConsistency();
            Assert.Equal(0, list.IndexOf(n));
        }

        [Fact]
        public void Test2()
        {
            var list = new WeightedList<BasicClass>().AddComparator((a, b) => a.Value.Value - b.Value.Value);

            list.Add(new BasicClass("a"){Value = 0});
            list.Add(new BasicClass("b"){Value = 0});
            list.Add(new BasicClass("c"){Value = 1});

            Assert.Equal(1, list.IndexOf(c => c.Name == "a"));
            Assert.Equal(2, list.IndexOf(c => c.Name == "b"));
            Assert.Equal(0, list.IndexOf(c => c.Name == "c"));
        }
    }
}
