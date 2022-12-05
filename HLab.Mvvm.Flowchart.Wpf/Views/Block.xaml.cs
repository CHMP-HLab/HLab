using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Extensions;
using HLab.Mvvm.Flowchart.ViewModel;


namespace HLab.Mvvm.Flowchart.Views
{
    /// <summary>
    /// Logique d'interaction pour Block.xaml
    /// </summary>
    public partial class BlockView : UserControl, 
        IView<ViewModeDefault,IBlockViewModel>,
        IView<ViewModeEdit,IBlockViewModel>
    {
        readonly IMessagesService _msg;

        public BlockView(IMessagesService msg)
        {
            _msg = msg;
            InitializeComponent();
            Loaded += Block_Loaded;
            Unloaded += Block_Unloaded;
        }

        void Block_Unloaded(object sender, RoutedEventArgs e)
        {
            _msg.Unsubscribe<PinViewModel>(SetDraggedPin);
        }

        void Block_Loaded(object sender, RoutedEventArgs e)
        {
            _msg.Subscribe<PinViewModel>(SetDraggedPin);
        }

        void SetDraggedPin(PinViewModel pvm)
        {
            ViewModel?.SetDraggedPin(pvm);
        }

        IBlockViewModel ViewModel => DataContext as IBlockViewModel;

        Point _oldMousePosition;

        void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement el)
            {
                el.CaptureMouse();
                _oldMousePosition = e.GetPosition(this.FindVisualParent<Grid>());
                _moved = false;
            }
        }

        void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement el && el.IsMouseCaptured)
            {
                if(!_moved) ViewModel.Select();
                el.ReleaseMouseCapture();
            }
        }

        bool _moved = false;

        void UIElement_OnMouseMove(object sender, MouseEventArgs e)
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

        void BlockView_OnLayoutUpdated(object sender, EventArgs e)
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
