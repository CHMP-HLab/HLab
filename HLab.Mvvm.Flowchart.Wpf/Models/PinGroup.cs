using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    public enum PinLocation { Left,Right }

    public interface IPinGroup : IGraphElement
    {
        ObservableCollection<IPin> Pins { get; }
        IGraphBlock Block { get; }
        PinLocation Location { get; set; }
        T GetOrAddPin<T>(string id = null, GraphValueType type = null) where T : IPin, new();
        T GetOrAddPin<T>(Action<T> factory, string id, GraphValueType type = null) where T:IPin, new();
//        T AddPin<T>(T pin) where T : IPin;

    }

    [DataContract]
    public class PinGroup : GraphElement, IPinGroup
    {
        public PinGroup(IGraphBlock block, string id, PinLocation location, string caption)
        {
            H<PinGroup>.Initialize(this);

            Block = block;
            Id = id;
            Location = location;
            Caption = caption;
        }

        public T GetOrAddPin<T>(string id = null, GraphValueType type = null) where T : IPin, new()
        {
            return GetOrAddPin<T>(null,id,type);
        }

        public T GetOrAddPin<T>(Action<T> factory, string id, GraphValueType type = null) where T:IPin, new()
        {
            if (string.IsNullOrEmpty(id))
            {
                var pins = Pins.Where(p => p.Id.StartsWith("#")).ToList();
                id = "#" + (pins.Count==0?0:pins.Max(p => int.Parse(p.Id.TrimStart('#'))) + 1);                
            }
            else
            {
                var pin = Pins.FirstOrDefault(p => p.Id == id);
                if (pin is T tp) return tp;
            }

            var newPin = new T
            {
                Id = id,
                Group = this,
            };

            if (type != null) newPin.ValueType = type;

            factory?.Invoke(newPin);

            Pins.Add(newPin);
            return newPin;
        }

        public IGraphBlock Block
        {
            get => _block.Get();
            set => _block.Set(value);
        }

        readonly IProperty<IGraphBlock> _block = H<PinGroup>.Property<IGraphBlock>(c => c.Default((IGraphBlock)default));



        [DataMember]
        public PinLocation Location
        {
            get => _location.Get();
            set => _location.Set(value);
        }

        readonly IProperty<PinLocation> _location = H<PinGroup>.Property<PinLocation>(c => c.Default(PinLocation.Left));



        [DataMember]
        public ObservableCollection<IPin> Pins { get; } =
            new ObservableCollection<IPin>();

    }
}
