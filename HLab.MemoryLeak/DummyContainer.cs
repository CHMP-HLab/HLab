using System.Linq;
using HLab.Notify.PropertyChanged;

namespace HLab.MemoryLeak
{
    class DummyContainer : NotifierBase
    {
        public DummyContainer() => H<DummyContainer>.Initialize(this);
        //public Dummy Dummy
        //{
        //    get => _dummy.Get();
        //    set => _dummy.Set(value);
        //}
        //private readonly IProperty<Dummy> _dummy = H<DummyContainer>.Property<Dummy>();

        public ObservableCollectionSafe<Dummy> Dummies { get; } = new ObservableCollectionSafe<Dummy>();


        public string Property => _property.Get();
        private readonly IProperty<string> _property = H<DummyContainer>.Property<string>( c => c
            .Set(e => e.Dummies.Max(d => d.Property))
            .On(e => e.Dummies.Item().Property).Update()
        );
    }
}