using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Observables;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    public abstract class GraphBlock<T> : GraphElement<T>, IGraphBlock
    where T : GraphBlock<T>
    {
        protected GraphBlock()
        {
            MainLeftGroup = GetOrAddGroup("Left", PinLocation.Left);
            MainRightGroup = GetOrAddGroup("Right", PinLocation.Right);
        }

        [Import] private Func<IGraphBlock,string, PinLocation, string, PinGroup> _getPinGroup;

        public IPinGroup GetOrAddGroup(string id, PinLocation location, string caption="")
        {
            // TODO : new to Ioc
            return Groups.GetOrAdd(g => g.Id == id,
                () => _getPinGroup(this,id,location,caption));
        }


        public ObservableCollection<IPin> TempPins { get; } = new ObservableCollection<IPin>();

        [DataMember]
        public ObservableCollection<IPinGroup> Groups { get; } = new ObservableCollection<IPinGroup>();


        public IPinGroup MainLeftGroup { get; }
        public IPinGroup MainRightGroup { get; }


        [DataMember]
        public double Left
        {
            get => _left.Get();
            set => _left.Set(value);
        }
        private readonly IProperty<double> _left = H.Property<double>(c => c.Default(0.0));


        [DataMember]
        public double Top
        {
            get => _top.Get();
            set => _top.Set(value);
        }
        private readonly IProperty<double> _top = H.Property<double>(c => c.Default(0.0));



        [DataMember]
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        private readonly IProperty<string> _name = H.Property<string>(c => c
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