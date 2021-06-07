using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HLab.Base.Wpf
{
    class HUserControl<T> : UserControl
    where T : DependencyObject
    {
        class H : DependencyHelper<T> {}
    }
}
