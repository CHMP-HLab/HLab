using System;
using System.Linq;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Observables;
using HLab.Notify.Annotations;

namespace HLab.Notify.UTests
{
    internal class LinkedCollectionTest : NotifierObject
    {
        [Import]
        public CollectionTest CollectionTest => N.Get<CollectionTest>();

        [Import] private Func<TestNotifierLevel0, TestNotifierLevel1> _getTestNotifierLevel0;

        [Import,TriggerOn(nameof(CollectionTest), "Collection")]
        
        public ObservableViewModelCollection<TestNotifierLevel1> LinkedCollection
        {
            get =>
                N.Get<ObservableViewModelCollection<TestNotifierLevel1>>();
            set => N.Set(value.AddCreator(e => _getTestNotifierLevel0(e as TestNotifierLevel0))
                        .Link(() => CollectionTest?.Collection));
        }


        [TriggerOn(nameof(LinkedCollection),"Item", "TestNotifierLevel0", "Value")]
        public int Sum => N.Get(() => LinkedCollection.Sum(e => e.TestNotifierLevel0.Value));


        public int Value
        {
            get => N.Get(() => -1); private set => N.Set(value);
        }

        [TriggerOn(nameof(LinkedCollection), "Item", "TestNotifierLevel0", "Value")]
        public void Test(object target, NotifierPropertyChangedEventArgs a)
        {
            Value = LinkedCollection.Sum(e => e.TestNotifierLevel0.Value);
        }
    }
}