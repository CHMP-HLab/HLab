using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Core;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.ViewModel;
using HLab.Mvvm.Wpf;


namespace HLab.Mvvm.Flowchart.Views
{
    /// <summary>
    /// Logique d'interaction pour Block.xaml
    /// </summary>
    public partial class BlockView : UserControl, 
        IView<ViewModeDefault,IBlockViewModel>,
        IView<ViewModeEdit,IBlockViewModel>
    {
        private readonly IMessageBus _msg;

        public BlockView(IMessageBus msg)
        {
            _msg = msg;
            InitializeComponent();
            Loaded += Block_Loaded;
            Unloaded += Block_Unloaded;
        }

        private void Block_Unloaded(object sender, RoutedEventArgs e)
        {
            _msg.Unsubscribe<PinViewModel>(SetDraggedPin);
        }

        private void Block_Loaded(object sender, RoutedEventArgs e)
        {
            _msg.Subscribe<PinViewModel>(SetDraggedPin);
        }

        private void SetDraggedPin(PinViewModel pvm)
        {
            ViewModel?.SetDraggedPin(pvm);
        }

        private IBlockViewModel ViewModel => DataContext as IBlockViewModel;

        private Point _oldMousePosition;
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement el)
            {
                el.CaptureMouse();
                _oldMousePosition = e.GetPosition(this.FindVisualParent<Grid>());
                _moved = false;
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement el && el.IsMouseCaptured)
            {
                if(!_moved) ViewModel.Select();
                el.ReleaseMouseCapture();
            }
        }

        private bool _moved = false;
        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement el && el.IsMouseCaptured)
            {
                var pos =  e.GetPosition(this.FindVisualParent<Grid>());

                var v = pos - _oldMousePosition;
                if (v.LengthSquared > 0)
                {
                    ViewModel.SetLocation(
                        Margin.Left + pos.X - _oldMousePosition.X,
                        Margin.Top + pos.Y - _oldMousePosition.Y);

                    _oldMousePosition = pos;
                    _moved = true;
                }
            }
        }

        private void BlockView_OnLayoutUpdated(object sender, EventArgs e)
        {
            if (ViewModel is BlockViewModel vm)
            {
                var editor = this.FindVisualParent<FlowchartEditorView>();
                if(editor!=null)
                    vm.Model.TempLocation = TranslatePoint(new Point(), editor.WorkArea);
            }
        }
    }
}
