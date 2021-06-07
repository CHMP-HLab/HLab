using System;
using System.Linq;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Observables;
using HLab.Notify.Annotations;

namespace HLab.Notify.UTests
{
    internal class SumCollection : NotifierObject
    {
        [Import]
        public LinkedCollectionTest LinkedCollectionTest
        {
            get => N.Get<LinkedCollectionTest>();
            set => N.Set(value);
        }

        [Import] private readonly Func<TestNotifierLevel1, TestNotifierLevel2> _getNewTestNotifierLevel2;

        [Import]
        [TriggerOn(nameof(LinkedCollectionTest), "LinkedCollection")]
        public ObservableViewModelCollection<TestNotifierLevel2> Collection
        {
            get =>
                N.Get<ObservableViewModelCollection<TestNotifierLevel2>>();
            set => N.Set(value.AddCreator(e => _getNewTestNotifierLevel2(e as TestNotifierLevel1))
                        .Link(() => LinkedCollectionTest?.LinkedCollection));
        }


        public int Value
        {
            get => N.Get(() => -1);
            private set => N.Set(value);
        }


        [TriggerOn(nameof(Collection), "Item", "TestNotifierLevel1", "TestNotifierLevel0", "Value")]
        public void Test()
        {
            Value = Collection.Sum(e => e.TestNotifierLevel1.TestNotifierLevel0.Value);
        }

        public override void OnSubscribe(INotifier n)
        {

        }
    }
}