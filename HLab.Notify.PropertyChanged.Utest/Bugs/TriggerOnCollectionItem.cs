using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest.Bugs
{

    public class Item : NotifierBase
    {
        public Item () => H<Item>.Initialize(this);

        public int Property {get => _property.Get(); set => _property.Set(value);}

        IProperty<int> _property = H<Item>.Property<int>();
    }

    public class TestClass :NotifierBase
    {
        public TestClass () => H<TestClass>.Initialize(this);

        public int Triggered {get; set;} = 0;

        public ObservableCollection<Item> Items {get; } = new ();

        ITrigger _ = H<TestClass>.Trigger(c => c
            .On(e => e.Items.Item().Property)
            .Do(e => e.Triggered++)
        );
    }

    public class TriggerOnCollectionItem
    {
        [Fact]
        public void DoTest()
        {
            var test = new TestClass();

            var item1 = new Item();
            var item2 = new Item();
            var item3 = new Item();
            var item4 = new Item();

            test.Items.Add(item1);
            test.Items.Add(item2);
            test.Items.Add(item3);
            test.Items.Add(item4);

            Assert.Equal(4,test.Triggered);

            item1.Property++;
            Assert.Equal(5,test.Triggered);
            item2.Property++;
            Assert.Equal(6,test.Triggered);
            item3.Property++;
            Assert.Equal(7,test.Triggered);
            item4.Property++;
            Assert.Equal(8,test.Triggered);

            test.Items.Remove(item4);
            item4.Property++;
            Assert.Equal(8,test.Triggered);

            test.Items.Remove(item3);
            item3.Property++;
            Assert.Equal(8,test.Triggered);

            test.Items.Add(item4);
            item4.Property++;
            Assert.Equal(9,test.Triggered);

        }

    }
}
