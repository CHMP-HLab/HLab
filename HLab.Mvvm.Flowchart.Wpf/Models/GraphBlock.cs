using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    using H = H<GraphBlock>; 

    public abstract class GraphBlock : GraphElement, IGraphBlock
    {
        public class Injector
        {
            public Func<IGraphBlock, string, PinLocation, string, PinGroup> GetPinGroup;
            public Injector(Func<IGraphBlock, string, PinLocation, string, PinGroup> getPinGroup)
            {
                GetPinGroup = getPinGroup;
            }
        }

        readonly Func<IGraphBlock,string, PinLocation, string, PinGroup> _getPinGroup;
        protected GraphBlock(Injector injector)
        {
            _getPinGroup = injector.GetPinGroup;
            MainLeftGroup = GetOrAddGroup("Left", PinLocation.Left);
            MainRightGroup = GetOrAddGroup("Right", PinLocation.Right);

            H.Initialize(this);
        }


        public IPinGroup GetOrAddGroup(string id, PinLocation location, string caption="")
        {
            return Groups.GetOrAdd(g => g.Id == id,
                () => _getPinGroup(this,id,location,caption));
        }


        public ObservableCollection<IPin> TempPins { get; } = new ObservableCollection<IPin>();

        [DataMember]
        public ObservableCollection<IPinGroup> Groups { get; } = new ObservableCollection<IPinGroup>();


        public IPinGroup MainLeftGroup { get; private set; }
        public IPinGroup MainRightGroup { get; private set; }


        [DataMember]
        public double Left
        {
            get => _left.Get();
            set => _left.Set(value);
        }

        readonly IProperty<double> _left = H.Property<double>(c => c.Default(0.0));


        [DataMember]
        public double Top
        {
            get => _top.Get();
            set => _top.Set(value);
        }

        readonly IProperty<double> _top = H.Property<double>(c => c.Default(0.0));



        [DataMember]
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c
            .On(e => e.Caption)
            .Set(e => e.Caption)
        );

        public abstract string IconName { get; }

        public IGraph Graph
        {
            get => (IGraph) Parent;
            set => Parent = value;
        }

        public Point TempLocation { get; set; }

        public virtual bool AskForInputPin(GraphValueType type)
        {
            return false;
        }

        public virtual void EndAskForInputPin()
        {
            foreach (var pin in TempPins.ToList())
            {
                if (!pin.IsLinked)
                {
                    foreach (var g in Groups)
                    {
                        if (g.Pins.Contains(pin)) g.Pins.Remove(pin);
                    }
                }
                
                if (TempPins.Contains(pin)) TempPins.Remove(pin);
            }
        }
    }
}