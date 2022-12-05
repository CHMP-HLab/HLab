using System.ComponentModel;
using System.Linq;
using HLab.Notify.PropertyChanged.UTest.Classes;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    public class UnitTestCollections
    {
        class ClassWithCollection : NotifierTest<ClassWithCollection>
        {
            public ObservableCollectionSafe<ClassWithProperty> Children { get; }
                = new ObservableCollectionSafe<ClassWithProperty>();

            public int Sum => _sum.Get();

            readonly IProperty<int> _sum = H.Property<int>(c => c
                .On(e => e.Children.Item().Value)
                .Set(e => e.Children.Sum(i => i.Value))
            );
        }
        class ClassWithCollectionWithChild : NotifierTest<ClassWithCollectionWithChild>
        {
            public ObservableCollectionSafe<ClassWithChild> Children { get; }
                = new ObservableCollectionSafe<ClassWithChild>();

            public int Sum => _sum.Get();

            readonly IProperty<int> _sum = H.Property<int>(c => c
                .On(e => e.Children.Item().Child)
                .Set(e => e.Children.Sum(i => i.Child.Value))
            );
        }

        [Fact]
        public void Test()
        {
            var c = new ClassWithCollection();
            var countItem = 0;
            var countCount = 0;
            var countOthers = 0;

            if (c.Children is INotifyPropertyChanged p)
            {
                p.PropertyChanged += (s, a) =>
                {
                    switch (a.PropertyName)
                    {
                        case "Item":
                            countItem++;
                            break;
                        case "Count":
                            countCount++;
                            break;
                        default:
                            countOthers++;
                            break;
                    }
                };

            }

            var child1 = new ClassWithProperty {Value = 25};
            c.Children.Add(child1);
            var child2 = new ClassWithProperty {Value = 17};
            c.Children.Add(child2);

            Assert.Equal(c.Children.Sum(i => i.Value), c.Sum);
            Assert.Equal(0,countOthers);
            //Assert.Equal(2, countCount);
            //Assert.Equal(2,countItem);

            child1.Value = 8;
            child2.Value = 2;
            Assert.Equal(c.Children.Sum(i => i.Value),c.Sum);
        }

        [Fact]
        public void TestWithChild()
        {
            var c = new ClassWithCollectionWithChild();

            var sum = 0;
            for (var i = 1; i < 10; i++)
            {
                c.Children.Add(new ClassWithChild{Child = new ClassWithProperty{Value = i}});
                sum += i;
                //GC.Collect();
                Assert.Equal(sum,c.Sum);
            }
        }
    }
}
