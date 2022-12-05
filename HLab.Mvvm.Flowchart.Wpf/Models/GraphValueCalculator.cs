using System;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    using H = H<GraphValueCalculator>;

    public class GraphValueCalculator : NotifierBase, ITriggerable
    {
        public double Value1 => _value1.Get();
        readonly IProperty<double> _value1 = H.Property<double>(c => c.Set(e => e.GetValue(1)));

        public double ValueN => _valueN.Get();
        readonly IProperty<double> _valueN = H.Property<double>(c => c.Set(e => e.GetValue(2) - e.Value1));
        public double Cost { get; set; } = 0;

        public Func<int, double> GetValue { get; set; }

        public void OnTriggered()
        {
            // TODO :
            //GetNotifier().GetEntry(nameof(Value1)).Update();
            //GetNotifier().GetEntry(nameof(ValueN)).Update();
        }
    }
}
