using HLab.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HLab.Base.Wpf
{
    using H = DependencyHelper<DateColumnItem>;
    public class DateColumnItem : TextBlock
    {
        public static readonly DependencyProperty DateProperty =
            H.Property<DateTime?>()
            .BindsTwoWayByDefault
            .OnChange((e, a) => e.SetDate(a.NewValue, e.DayValid)).Register();

        public static readonly DependencyProperty DayValidProperty =
            H.Property<bool>()
            .BindsTwoWayByDefault
            .OnChange((e, a) => e.SetDate(e.Date, a.NewValue)).Register();

        public DateTime? Date
        {
            get => ((DateTime?)GetValue(DateProperty)).ToLocalTime();
            set => SetValue(DateProperty, value.ToLocalTime());
        }

        public bool DayValid
        {
            get => (bool)GetValue(DayValidProperty);
            set => SetValue(DayValidProperty, value);
        }
        void SetDate(DateTime? date, bool dayValid)
        {
            if (date.HasValue)
            {
                var d = date.Value.ToLocalTime();

                Text = d.ToString(dayValid ? "dd/MM/yyyy" : "MM/yyyy");
            }
            else
            {
                Text = "";
            }
        }
    }
}
