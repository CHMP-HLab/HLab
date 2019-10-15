using System;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.Models
{
    public class GraphValueCalculator : N<GraphValueCalculator>, ITriggable
    {
        public double Value1 => _value1.Get();
        private readonly IProperty<double> _value1 = H.Property<double>(c => c.Set(e => e.GetValue(1)));

        public double ValueN => _valueN.Get();
        private readonly IProperty<double> _valueN = H.Property<double>(c => c.Set(e => e.GetValue(2) - e.Value1));
        public double Cost { get; set; } = 0;

        public Func<int, double> GetValue { get; set; }
        public void OnTrigged()
        {
            // TODO :
            //GetNotifier().GetEntry(nameof(Value1)).Update();
            //GetNotifier().GetEntry(nameof(ValueN)).Update();
        }
    }
}
