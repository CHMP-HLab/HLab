using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    public interface IInputPin : IPin
    {
        IOutputPin LinkedOutput { get; set; }
    }



    [DataContract]
    public abstract class InputPin<T> : Pin<T>, IInputPin
    where T : InputPin<T>
    {
        public override Color Color => _color.Get();
        private readonly IProperty<Color> _color = H.Property<Color>(c => c
            .On(e => e.ValueType.Color)
            .Set(e => e.ValueType.Color));

        public override PinDirection Direction => PinDirection.Input;


        //public  Func<int,double> GetValue
        //{
        //    get => this.Get<Func<int,double>>();
        //    set => N.Set(value);
        //}

        public override double GetValue(int n) => 0.0;
        public IOutputPin LinkedOutput
        {
            get => _linkedOutput.Get();
            set
            {

                var oldValue = _linkedOutput.Get();

                if (_linkedOutput.Set(value))
                {
                    oldValue?.LinkedPins.Remove(this);
                    value?.LinkedPins.Add(this);
                }


                //_linkedOutput.SetOneToMany(value, o => o.LinkedPins);
            }
        }

        private readonly IProperty<IOutputPin> _linkedOutput = H.Property<IOutputPin>(c => c.Default(default));


        [DataMember]
        public string LinkedOutputPath
        {
            get => _linkedOutputPath.Get();
            set => _linkedOutputPath.Set(value);
        }

        private readonly IProperty<string> _linkedOutputPath = H.Property<string>(c => c
            .Set(e => e.LinkedOutput.ToString())
        );


        [TriggerOn(nameof(LinkedOutputPath))]
        public void UpdateLink()
        {
            var graph = Group.Block.Graph;
            graph.BlocksLoaded += (a, b) => { SetLinkFromPath(); };
        }

        public void SetLinkFromPath()
        {
            var path = LinkedOutputPath.Split('/');
            if (path.Length == 3)
            {
                var graph = Group.Block.Graph;
                var block = graph.Blocks.FirstOrDefault(b => b.Id == path[0]);
                var group = block?.Groups.FirstOrDefault(g => g.Id == path[1]);
                var pin = group?.Pins.FirstOrDefault(p => p.Id == path[2]);
                if(pin is IOutputPin op)
                    LinkedOutput = op;
            }
        }

        public override bool IsLinked => _isLinked.Get();
        private readonly IProperty<bool> _isLinked = H.Property<bool>(c => c
            .On(e => e.LinkedOutput)
            .Set(e => e.LinkedOutput != null));

    }
}
