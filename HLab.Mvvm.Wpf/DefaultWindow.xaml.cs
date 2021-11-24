using HLab.Base.Wpf;

using System;
using System.Windows;
using System.Windows.Input;

namespace HLab.Mvvm
{
    using H = DependencyHelper<DefaultWindow>;

    /// <summary>
    /// Logique d'interaction pour DefaultWindow.xaml
    /// </summary>
    public partial class DefaultWindow
    {
        public DefaultWindow()
        {
            InitializeComponent();
        }
        public object View
        {
            get => (object)GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        public static readonly DependencyProperty ViewProperty =
            H.Property<object>()
                .OnChange((e, a) =>
                {
                    e.ViewContent.Content = a.NewValue;
                })
                .Register();


        private bool _clicked = false;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);

            //Not in drag zone  (Title bar)
            if (pos.Y > 30) return;
            if(e.ClickCount>1)
            {
                if(WindowState == WindowState.Normal) 
                    WindowState = WindowState.Maximized; 
                else WindowState = WindowState.Normal;

                return;
            }

            _clicked = true;

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_clicked) return;

            var pos = e.GetPosition(this);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == WindowState.Maximized)
                {
                    var width = ActualWidth;
                    var height = ActualHeight;


                    var xRatio = pos.X / width;

                    var absPos = PointToScreen(pos);

                    var ct = PresentationSource.FromVisual(this)?.CompositionTarget;

                    if (ct != null)
                    {
                        var m = ct.TransformToDevice;


                        Top = (absPos.Y / m.M22) - pos.Y * (Height / height);

                        Left = (absPos.X / m.M11) - pos.X * (Width / width);

                        WindowState = WindowState.Normal;
                    }

                }

                _clicked = false;
                try
                {
                    DragMove();
                }
                catch (InvalidOperationException) { }
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _clicked = false;
        }


        CornerRadius _cornerRadius = new(0.0);
        CornerRadius _cornerRadiusZero = new(0.0);
        protected override void OnStateChanged(EventArgs e)
        {
            if (_cornerRadius == _cornerRadiusZero) _cornerRadius = Border.CornerRadius;

            switch (WindowState)
            {
                case WindowState.Normal:
                    if (_cornerRadius != _cornerRadiusZero)
                        Border.CornerRadius = _cornerRadius;

                    Border.BorderThickness = new Thickness(1.0);
                    break;

                case WindowState.Maximized:
                    if (_cornerRadius != _cornerRadiusZero)
                        Border.CornerRadius = _cornerRadiusZero;

                    Border.BorderThickness = new Thickness(0.0);
                    break;

                case WindowState.Minimized:
                    if (_cornerRadius != _cornerRadiusZero)
                        Border.CornerRadius = _cornerRadiusZero;
                    break;
            }
            base.OnStateChanged(e);
        }
    }
}
