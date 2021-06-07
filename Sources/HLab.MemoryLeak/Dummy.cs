using System;
using System.Collections.Generic;
using System.Text;
using HLab.Notify.PropertyChanged;

namespace HLab.MemoryLeak
{
    class Dummy : NotifierBase
    {
        public Dummy() => H<Dummy>.Initialize(this);
        public string Property
        {
            get => _property.Get();
            set => _property.Set(value);
        }
        private readonly IProperty<string> _property = H<Dummy>.Property<string>();
    }
}
