using System.Windows.Controls;
using System.Windows.Input;

namespace HLab.Base.Wpf.ControlsEx
{
    public class ScrollViewerEx : ScrollViewer
    {
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            ScrollToVerticalOffset(VerticalOffset - e.Delta);
            e.Handled = true;
            base.OnPreviewMouseWheel(e);
        }
    }
}
