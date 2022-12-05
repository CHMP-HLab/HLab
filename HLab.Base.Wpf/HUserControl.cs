using System.Windows;
using System.Windows.Controls;

namespace HLab.Base.Wpf
{
    internal class HUserControl<T> : UserControl
    where T : DependencyObject
    {
        class H : DependencyHelper<T> {}
    }
}
