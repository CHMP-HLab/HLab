using HLab.DependencyInjection.Annotations;
using HLab.Notify.Collections;
using HLab.Notify.Annotations;

namespace HLab.Notify.UTests
{
    internal class CollectionTest : NotifierObject
    {
        [Import]
        public ObservableCollectionNotifier<TestNotifierLevel0> Collection
        {
            get => N.Get<ObservableCollectionNotifier<TestNotifierLevel0>>();
            private set => N.Set(value);
        }
    }
}