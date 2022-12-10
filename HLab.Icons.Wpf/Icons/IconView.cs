using HLab.Base.Wpf;
using HLab.Icons.Annotations.Icons;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using SystemColors = System.Windows.SystemColors;

namespace HLab.Icons.Wpf.Icons;

using H = DependencyHelper<IconView>;

[ContentProperty("Caption")]
public class IconView : ContentControl
{
#if DEBUG
    static IIconService _designTimeService = null;
#endif
    readonly ContentPresenter _iconElement = new() { /*IsTabStop = false, */VerticalAlignment = VerticalAlignment.Center };
    readonly ContentPresenter _captionElement = new() { /*IsTabStop = false, */VerticalAlignment = VerticalAlignment.Center };
    readonly ColumnDefinition _column0 = new() { Width = GridLength.Auto };
    static readonly GridLength GridLength0 = new(0.0);
    static readonly GridLength GridLength10 = new(10.0);
    readonly ColumnDefinition _column1 = new() { Width = GridLength0 };
    static readonly GridLength GridLengthStar = new(1.0, GridUnitType.Star);
    readonly ColumnDefinition _column2 = new() { Width = GridLengthStar };

    public IconView()
    {
        _captionElement.SetValue(Grid.ColumnProperty, 2);
        //Canvas iconCanvas = new()
        //{
        //    Children = {_iconElement}
        //};

        Content = new Grid
        {
            ColumnDefinitions = {
                _column0,
                _column1,
                _column2,
            },

            Children =
            {
                _iconElement,
                _captionElement
            }
        };

        //Loaded += IconView_Loaded;
    }

    //void IconView_Loaded(object sender, RoutedEventArgs e)
    //{
    //    Update();
    //}

    /// <summary>
    /// IconService
    /// </summary>
    public static readonly DependencyProperty IconServiceProperty =
        H.Property<IIconService>()
            .OnChange((e, a) =>
            {
                e.LoadIcon(e.Path);
            })
            .Inherits
            .RegisterAttached();

    public static IIconService GetIconService(DependencyObject obj)
    {
        return (IIconService)obj.GetValue(IconServiceProperty);
    }

    public static void SetIconService(DependencyObject obj, IIconService value)
    {
        obj.SetValue(IconServiceProperty, value);
    }

    public static readonly DependencyProperty PathProperty =
        H.Property<string>()
            .OnChange((e, a) =>
            {
                e.LoadIcon(a.NewValue);
            })
            .Register();

    public static readonly DependencyProperty CaptionProperty =
        H.Property<object>()
            .OnChange((s, e) =>
            {
                s._captionElement.Content = e.NewValue;
                s.Update();
            })
            .Register();

    public static readonly DependencyProperty IconMaxHeightProperty =
        H.Property<double>().Default(30.0).OnChange((e, a) =>
            {
                if (double.IsNaN(a.NewValue)) return;
                e._iconElement.MaxHeight = a.NewValue;
            })
            .Register();

    public static readonly DependencyProperty IconMaxWidthProperty =
        H.Property<double>().Default(50.0).OnChange((e, a) =>
            {
                if (double.IsNaN(a.NewValue)) return;
                e._column0.Width = new GridLength(a.NewValue);
                e._iconElement.MaxWidth = a.NewValue;
            })
            .Register();


    public string Path
    {
        get => (string)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    public object Caption
    {
        get => (object)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }
    public double IconMaxHeight
    {
        get => (double)GetValue(IconMaxHeightProperty);
        set => SetValue(IconMaxHeightProperty, value);
    }
    public double IconMaxWidth
    {
        get => (double)GetValue(IconMaxWidthProperty);
        set => SetValue(IconMaxWidthProperty, value);
    }

    public IIconService IconService
    {
        get => (IIconService)GetValue(IconServiceProperty);
        set => SetValue(IconServiceProperty, value);
    }
    class Canceler
    {
        public bool State { get; private set; }

        public void Cancel()
        {
            State = true;
        }
    }
    readonly ConcurrentStack<Canceler> _cancel = new();

    void LoadIcon(string path)
    {
        if(path==null) return;
        if (IconService == null)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                if (_designTimeService == null)
                {
                    _designTimeService = new IconService();
                    new IconBootloader(_designTimeService).Load(null);
                }

                IconService = _designTimeService;
            }
            else
#endif
                return;
        }

        var iconService = IconService;

        while (_cancel.TryPop(out var c))
        {
            c.Cancel();
        }
        var cancel = new Canceler();
        _cancel.Push(cancel);

        var t2 = Dispatcher.BeginInvoke(
            async () =>
            {

                if (cancel.State) return;
                var icon = await iconService.GetIconTemplateAsync(path);
                if (cancel.State) return;
                if (icon is DataTemplate template)
                {
                    _iconElement.ContentTemplate = template;
                    Update();
                }
            }
            , XamlTools.Priority);

    }

    void Update()
    {
        if (_iconElement.ContentTemplate==null)
        {
            _iconElement.Visibility = Visibility.Collapsed;
            _column1.Width = GridLength0;

            switch (Caption)
            {
                case null:
                case string c when string.IsNullOrWhiteSpace(c):
                    _captionElement.Visibility = Visibility.Collapsed;
                    break;
                default:
                    _captionElement.Content = Caption;
                    _captionElement.Visibility = Visibility.Visible;
                    break;
            }
        }
        else
        {
            _iconElement.Visibility = Visibility.Visible;
            _column1.Width = GridLength.Auto;

            switch (_captionElement.Content)
            {
                case null:
                case string c when string.IsNullOrWhiteSpace(c):
                    _captionElement.Visibility = Visibility.Collapsed;
                    _column1.Width = GridLength0;
                    break;
                default:
                    _captionElement.Visibility = Visibility.Visible;
                    _column1.Width = GridLength10;
                    break;
            }
        }
    }

}