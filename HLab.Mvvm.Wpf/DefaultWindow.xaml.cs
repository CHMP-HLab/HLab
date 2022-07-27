using HLab.Base.Wpf;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HLab.Mvvm;

using H = DependencyHelper<DefaultWindow>;

/// <summary>
/// Logique d'interaction pour DefaultWindow.xaml
/// </summary>
public partial class DefaultWindow
{
    readonly Border _insideBorder;
    readonly ContentControl _content;

    public DefaultWindow()
    {
        InitializeComponent();

        if (ResizeGrid.NestedContent is Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Border border) _insideBorder = border;
                if (child is ContentControl content) _content = content;
            }
        }
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
                e._content.Content = a.NewValue;
            })
            .Register();


    bool _clicked = false;

    void OnMouseDown(object sender, MouseButtonEventArgs e)
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

    void OnMouseMove(object sender, MouseEventArgs e)
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

    void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _clicked = false;
    }


    CornerRadius _cornerRadius = new(0.0);
    CornerRadius _cornerRadiusZero = new(0.0);

    bool _maximize = false;

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);

        if (_cornerRadius == _cornerRadiusZero) _cornerRadius = _insideBorder.CornerRadius;

        switch (WindowState)
        {
            case WindowState.Normal:
                if (_cornerRadius != _cornerRadiusZero)
                    _insideBorder.CornerRadius = _cornerRadius;

                _insideBorder.BorderThickness = new Thickness(1.0);
                break;

            case WindowState.Maximized:
                if (_cornerRadius != _cornerRadiusZero)
                    _insideBorder.CornerRadius = _cornerRadiusZero;

                _insideBorder.BorderThickness = new Thickness(0.0);

                if(!_maximize)
                {
                    _maximize = true;
                    WindowState = WindowState.Minimized;
                }
                else _maximize = false;

                break;

            case WindowState.Minimized:

                if(_maximize)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    if (_cornerRadius != _cornerRadiusZero)
                        _insideBorder.CornerRadius = _cornerRadiusZero;
                }

                break;
        }
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        return base.ArrangeOverride(arrangeBounds);
    }
}
