using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using PinDirection = HLab.Mvvm.Flowchart.Models.PinDirection;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    using H = H<PinViewModel>;
    public class PinViewModel : ViewModel<IPin>, IPinViewModel
    {

        public PinViewModel(IMessagesService messageBus, IGraphService graphService)
        {
            MessageBus = messageBus;
            GraphService = graphService;
            
            H.Initialize(this);
        }
#if(DEBUG)
        //public IPin GetDesignModel => new InputPin(null,"0");
#endif
        public Brush Brush => _brush.Get();

        readonly IProperty<Brush> _brush = H.Property<Brush>(c => c
            .On(e => e.Model.Color)
            .Set(e => (Brush)new SolidColorBrush(e.Model.Color)));


        public Brush BackgroundBrush => _backgroundBrush.Get();

        readonly IProperty<Brush> _backgroundBrush = H.Property<Brush>(c => c
            .On(e => e.Brush)
            .On(e => e.Model.IsLinked)
            .Set(e => e.Model.IsLinked ? e.Brush : new SolidColorBrush(Colors.Transparent)));


         public Point LinkPoint
        {
            get => _linkPoint.Get();
            set => _linkPoint.Set(value);
        }

         readonly IProperty<Point> _linkPoint = H.Property<Point>(c => c.Default(new Point()));


        public IInputPin InputPin => _inputPin.Get();

        readonly IProperty<IInputPin> _inputPin = H.Property<IInputPin>(c => c
            .On(e => e.Model)
            .Set(e => e.Model as IInputPin)
            );


        public PinViewModel LinkedOutputViewModel => _linkedOutputViewModel.Get();

        readonly IProperty<PinViewModel> _linkedOutputViewModel = H.Property<PinViewModel>(c => c
            .On(e => e.InputPin.LinkedOutput)
            .Set(e => e.InputPin?.LinkedOutput == null?null:(PinViewModel) e.MvvmContext.GetLinked<ViewModeDefault>(e.InputPin.LinkedOutput))
            );

        public Path Path => _path.Get();

        readonly IProperty<Path> _path = H.Property<Path>(c => c
            .Set(
                e => new Path
        {
            StrokeThickness = 3,
            IsHitTestVisible = false,
            Stroke = e.Brush,
            Data = new PathGeometry
            {
                Figures =
                {
                    new PathFigure
                    {
                        Segments = {new LineSegment(), new QuadraticBezierSegment(),new QuadraticBezierSegment(), new LineSegment()},
                    }
                }
            }
        }
                
                )
        
        );

        // TODO : put that in Property
        [TriggerOn(nameof(LinkPoint))]
        [TriggerOn(nameof(LinkedOutputViewModel),"LinkPoint")]
        public void SetPath(/*object s, PropertyChangedEventArgs a*/)
        {
            if (LinkedOutputViewModel == null)
            {
                Path.Visibility = Visibility.Hidden;
                return;
            }

            var  endPoint2 = LinkPoint;
            var startPoint2 = LinkedOutputViewModel.LinkPoint;

            var shift = new Vector(10, 0);

            var endPoint = endPoint2 - shift;
            var startPoint = startPoint2 + shift;

            var path = Path;


            if (!(path?.Data is PathGeometry pg)) return;

            var pf = pg.Figures[0];

            var w = (startPoint.X < endPoint.X) ? (endPoint.X - startPoint.X) / 3 : (startPoint.X - endPoint.X) / 2;
            var h = (startPoint.X < endPoint.X) ? 0 : (endPoint.Y - startPoint.Y) / 4;
            //var w = (endPoint.X - startPoint.X) / 3;
            //var h = 0;
            pf.StartPoint = startPoint2;

            ((LineSegment)pf.Segments[0]).Point = startPoint;

            ((QuadraticBezierSegment)pf.Segments[1]).Point1 = new Point(startPoint.X + w, startPoint.Y + h);
            ((QuadraticBezierSegment)pf.Segments[1]).Point2 = new Point((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);
            ((QuadraticBezierSegment)pf.Segments[2]).Point1 = new Point(endPoint.X - w, endPoint.Y - h);

            ((QuadraticBezierSegment)pf.Segments[2]).Point2 = endPoint;
            ((LineSegment)pf.Segments[3]).Point = endPoint2;

            path.Visibility = Visibility.Visible;
        }

        public Visibility LeftIconVisibility =>
            Model.Direction == PinDirection.Input ? Visibility.Visible : Visibility.Collapsed;
        public Visibility RightIconVisibility =>
            Model.Direction == PinDirection.Output ? Visibility.Visible : Visibility.Collapsed;

        public double Opacity => _opacity.Get();

        readonly IProperty<double> _opacity = H.Property<double>(c => c
            .On(e => e.Enabled)
            .Set(e => e.Enabled?1.0:0.3));



        public bool IsConnectable(PinViewModel destViewModel)
        {
            if(Model.Direction == destViewModel.Model.Direction) return false;
            if (Model.ValueType != destViewModel.Model.ValueType) return false;
            return true;
        }

        public bool Connect(PinViewModel destViewModel)
        {
            if (IsConnectable(destViewModel))
            {
                if (Model is IInputPin i)
                {
                    i.LinkedOutput = (IOutputPin)destViewModel.Model;
                    return true;
                }
                if (Model is IOutputPin o)
                {
                    ((IInputPin) destViewModel.Model).LinkedOutput = o;
                    return true;
                }
            }
            return false;
        }

        public bool Enabled
        {
            get => _enabled.Get();
            set => _enabled.Set(value);
        }

        readonly IProperty<bool> _enabled = H.Property<bool>(c => c.Default((bool)default));


        public IMessagesService MessageBus { get; }
        public IGraphService GraphService { get; }

        public void SetDraggedPin(PinViewModel pvm)
        {
            Enabled = pvm == null || IsConnectable(pvm);
        }
    }

}
