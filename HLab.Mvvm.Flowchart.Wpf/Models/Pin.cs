using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    public abstract class Pin : GraphElement, IPin
    {
        protected Pin() => H<Pin>.Initialize(this);

        [TriggerOn(nameof(Parent))]
        public IPinGroup Group
        {
            get => Parent as IPinGroup;
            set => Parent = value;
        }

        public ObservableCollection<IPin> LinkedPins { get; } = new ObservableCollection<IPin>(); // => N<>.Get(() => );

        public virtual double Value => _value.Get();
        readonly IProperty<double> _value = H<Pin>.Property<double>(c => c.Set(e => e.GetValue(1)));


        public virtual double GetValue(int n)
        {
            throw new NotImplementedException();
        }

        [DataMember]
        public virtual PinDirection Direction { get; set; }

        public virtual bool IsLinked => false;

        [TriggerOn(nameof(IsLinked))]
        public void UpdateLinked()
        {
            if (!IsLinked && (Id?.StartsWith("#")??false))
            {
                Group.Pins.Remove(this);
                Group = null;
            }
        }
    




    [DataMember]
        public virtual GraphValueType ValueType { get; set; }

        public override string ToString() => Group.Block.Id +
                                             "/" + Group.Id + 
                                             "/" + Id;

    }


}
