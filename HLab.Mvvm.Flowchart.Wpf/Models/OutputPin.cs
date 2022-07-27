using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    using H = H<OutputPin>;

    public interface IOutputPin : IPin
    {
        //ObservableCollection<IInputPin> LinkedInputs { get; }
    }

    public interface IGraphDataSet
    {
        
    }

    [DataContract]
    public class OutputPin : Pin, IOutputPin
    {
        public OutputPin() => H.Initialize(this);

        //public ObservableCollection<IInputPin> LinkedInputs => N.Get(() => new ObservableCollection<IInputPin>());

        [TriggerOn(nameof(LinkedPins),"Item","Value")]
        public override double Value => base.Value;
        public override double GetValue(int n) => LinkedPins.Sum(p => p.GetValue(n));

        public override bool IsLinked => _isLinked.Get();

        readonly IProperty<bool> _isLinked = H.Property<bool>(c => c
            .On(e => e.LinkedPins.Item())
            .Set(e => e.LinkedPins.Count > 0));


        public override Color Color => _color.Get();

        readonly IProperty<Color> _color = H.Property<Color>(c => c
            .On(e => e.ValueType.Color)
            .Set(e => e.ValueType.Color));


        public override PinDirection Direction => PinDirection.Output;
    }
}
