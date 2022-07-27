using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Extensions;
using HLab.Mvvm.Flowchart.Models;
using HLab.Mvvm.Flowchart.ViewModel;

namespace HLab.Mvvm.Flowchart.Views
{
    public enum PinDirection
    {
        Input,
        Output
    }


    /// <summary>
    /// Logique d'interaction pour OutputPinView.xaml
    /// </summary>
    public partial class PinView : UserControl, 
        IView<ViewModeDefault, IPinViewModel>,
        IView<ViewModeEdit, IPinViewModel>
    {
        public PinView()
        {
            InitializeComponent();
            Loaded += PinView_Loaded;
            Unloaded += PinView_Unloaded;
        }

        void PinView_Unloaded(object sender, RoutedEventArgs e)
        {
            LayoutUpdated -= PinView_LayoutUpdated;
            //DragCanvas.Children.Remove(_path);
            ViewModel.MessageBus.Unsubscribe<PinViewModel>(SetDraggedPin);
        }

        Path _path;

        void PinView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            SetLinkedPoint();
            LayoutUpdated += PinView_LayoutUpdated; ;
            _path = ViewModel.Path;
            if (!DragCanvas.Children.Contains(_path))
            {
                if (_path.Parent is Panel p)
                {
                    p.Children.Remove(_path);
                }

                DragCanvas.Children.Add(_path);
            }

            ViewModel.MessageBus.Subscribe<PinViewModel>(SetDraggedPin);
        }

        void SetDraggedPin(PinViewModel pvm)
        {
                ViewModel?.SetDraggedPin(pvm);            
        }


        void PinView_LayoutUpdated(object sender, System.EventArgs e)
        {
            SetLinkedPoint();
        }

        void SetLinkedPoint()
        {
            if (ViewModel == null) return;
            var icon = LeftIcon.Visibility == Visibility.Visible ? LeftIcon : RightIcon;
            ViewModel.LinkPoint = icon.TranslatePoint(new Point(icon.ActualWidth / 2, icon.ActualHeight / 2), DragCanvas);
        }


        PinViewModel ViewModel => DataContext as PinViewModel;
        PinViewModel StartViewModel => ViewModel?.LinkedOutputViewModel??ViewModel;

        void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement el)
            {
                el.CaptureMouse();


                SetPath();
                if (!DragCanvas.Children.Contains(_dragPath))
                    DragCanvas.Children.Add(_dragPath);

                ViewModel.MessageBus.Publish(StartViewModel);
            }
        }

        Grid DragCanvas => this.FindVisualParent<FlowchartEditorView>()?.LinkArea;
        Grid WorkCanvas => this.FindVisualParent<FlowchartEditorView>()?.WorkArea;

        void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement el)
            {
                if(DragCanvas.Children.Contains(_dragPath))
                    DragCanvas.Children.Remove(_dragPath);

                _dragPath = null;

                el.ReleaseMouseCapture();

                var vm = StartViewModel;

                if (ViewModel.Model is IInputPin pin && pin.LinkedOutput != null)
                    pin.LinkedOutput = null;

                if (_dest?.ViewModel != null)
                {

                    vm.Connect(_dest.ViewModel);

                    var b1 = vm.Model.Group.Block;
                    var b2 = _dest.ViewModel.Model.Group.Block;

                    var toolblocks = ViewModel.GraphService.Blocks;

                    if (b1 is IToolGraphBlock bb1 && toolblocks.Contains(bb1))
                    {
                        AddBlock(b2.Graph,bb1);
                    }
                    if (b2 is IToolGraphBlock bb2 && toolblocks.Contains(bb2))
                    {
                        AddBlock(b1.Graph,bb2);
                    }

                    _dest = null;
                }

                ViewModel.MessageBus.Publish<PinViewModel>(null);
            }
        }

        void AddBlock(IGraph graph, IToolGraphBlock block)
        {
            this.FindVisualParent<FlowchartEditorView>().HideToolbox();
            var toolblocks = ViewModel.GraphService.Blocks;
            var i = toolblocks.IndexOf(block);

            block.Left = block.TempLocation.X;
            block.Top = block.TempLocation.Y;

            graph.AddBlock(block);

            var newBlock = (IToolGraphBlock)Activator.CreateInstance(block.GetType());
            toolblocks.Insert(i,newBlock);
            toolblocks.Remove(block);

        }

        bool IsConnectable(PinView dest)
        {
            return StartViewModel.IsConnectable(dest.ViewModel);
        }

        PinView _dest = null;

        void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is UIElement el && el.IsMouseCaptured)
            {
                _endPoint = e.GetPosition(WorkCanvas);

                SetPath();

                var hit = HitTestHelper.HitTest(WorkCanvas, _endPoint)?.VisualHit;

//                Debug.Print(hit?.GetType()?.Name ?? "-");
                var dest = hit?.FindVisualParent<PinView>();
//                Debug.Print(dest==null?"-":"x");
                if (dest == null || IsConnectable(dest))
                {
                    _dest = dest;
                }
                else _dest = null;
            }
        }

        Path _dragPath = null;

        Point _endPoint;

        void SetPath()
        {

            var startPoint = StartViewModel.Model.Direction == Models.PinDirection.Output ? StartViewModel.LinkPoint : _endPoint;
            var endPoint = StartViewModel.Model.Direction == Models.PinDirection.Output ? _endPoint : StartViewModel.LinkPoint;

            if (_dragPath == null)
            {
                _dragPath = new Path
                {
                    StrokeThickness = 3,
                    IsHitTestVisible = false,
                    Stroke = ViewModel.Brush,
                    Data = new PathGeometry
                    {
                        Figures =
                        {
                            new  PathFigure
                            {
                                Segments = {new QuadraticBezierSegment(), new QuadraticBezierSegment()},
                            }
                        }
                    }
                };
            }

            if (!(_dragPath.Data is PathGeometry pg)) return;

            var pf = pg.Figures[0];

            var w = (startPoint.X < endPoint.X) ? (endPoint.X - startPoint.X) / 3 : (startPoint.X - endPoint.X) / 2;
            var h = (startPoint.X < endPoint.X) ? 0 : (endPoint.Y - startPoint.Y) / 4;
            //var w = (endPoint.X - startPoint.X) / 3;
            //var h = 0;
            pf.StartPoint = startPoint;

            ((QuadraticBezierSegment)pf.Segments[0]).Point1 = new Point(startPoint.X + w, startPoint.Y + h);
            ((QuadraticBezierSegment)pf.Segments[0]).Point2 = new Point((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);
            ((QuadraticBezierSegment)pf.Segments[1]).Point1 = new Point(endPoint.X - w, endPoint.Y - h);

            ((QuadraticBezierSegment)pf.Segments[1]).Point2 = endPoint;
        }

        void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            BorderMouseMove.Background = ViewModel.Brush;
        }

        void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            BorderMouseMove.Background = null;
        }
    }
}
