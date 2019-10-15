using System.Windows;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    public interface IBlockViewModel
    {
        IGraphViewModel GraphViewModel { get; set; }
        Thickness Margin { get; }
        void SetLocation(double left, double top);

        bool Selected { get; set; }

        void Select();

        void SetDraggedPin(PinViewModel pvm);
    }
}
