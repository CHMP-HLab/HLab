using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using HLab.Base.Avalonia.DependencyHelpers;
using HLab.ColorTools.Avalonia;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Avalonia.Icons;

using H = DependencyHelper<IconView>;


public class IconView : ContentControl
{
    protected override Type StyleKeyOverride => typeof(ContentControl);

    #if DEBUG
    static IIconService? _designTimeService;
    #endif

    readonly ContentControl _iconElement = new() { IsTabStop = false, VerticalAlignment = VerticalAlignment.Center };
    readonly ContentControl _captionElement = new() { IsTabStop = false, VerticalAlignment = VerticalAlignment.Center };
    readonly ColumnDefinition _spacer = new() { Width = new GridLength(0.0) };

    public IconView()
    {
        _captionElement.SetValue(Grid.ColumnProperty, 2);

        Content = new Grid
        {
            ColumnDefinitions = {
                new ColumnDefinition{ Width = GridLength.Auto},
                _spacer,
                new ColumnDefinition{ Width = new GridLength(1.0,GridUnitType.Star) },
            },

            Children = { _iconElement, _captionElement }
        };

        AttachedToVisualTree += IconView_Loaded;

        PropertyChanged += OnPropertyChanged;
    }

    //Rebuild icon on foreground change (Full black is replaced by foreground color)
    static void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if(sender is not IconView iconView) return;
        if (e.Property == ForegroundProperty)
        {
            iconView.LoadIcon();
        }
    }

    static IconView()
    {

        /* TODO : Avalonia
        ForegroundProperty.OverrideMetadata<IconView>(typeof(IconView), new FrameworkPropertyMetadata(SystemColors.ControlTextBrush,
                        FrameworkPropertyMetadataOptions.Inherits, (s, e) => ((IconView)s).OnForegroundChanged(e)));

        == First attempt : ==


        ForegroundProperty.OverrideMetadata<IconView>(new StyledPropertyMetadata<IBrush?>());

        */

        //AffectsRender<ContentPresenter>(CaptionProperty, IconServiceProperty, PathProperty, IconMaxHeightProperty, IconMaxWidthProperty);
        //AffectsArrange<ContentPresenter>(CaptionProperty, IconServiceProperty, PathProperty, IconMaxHeightProperty, IconMaxWidthProperty);
        //AffectsMeasure<ContentPresenter>(CaptionProperty, IconServiceProperty, PathProperty, IconMaxHeightProperty, IconMaxWidthProperty);
    }

    bool _attached = false;
    void IconView_Loaded(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
    {
        _attached = true;
        LoadIcon();
    }


    /// <summary>
    /// IconService
    /// </summary>
    public static readonly StyledProperty<IIconService?> IconServiceProperty =
        H.Property<IIconService?>()
            .OnChanged(async (e,a) =>
            {
                //if (a.NewValue == null) return;
                //if (e._iconServiceReference != null && e._iconServiceReference.TryGetTarget(out var iconService) && ReferenceEquals(a.NewValue, iconService)) return;

                //e._iconServiceReference = new(a.NewValue);

                //if (e.Path == null) return;

                //await e.LoadIconAsync().ConfigureAwait(false);
            })
            .Inherits
            .Attached
            .Register();

    public static IIconService GetIconService(StyledElement obj)
    {
        return obj.GetValue(IconServiceProperty);
    }

    public static void SetIconService(StyledElement obj, IIconService value)
    {
        obj.SetValue(IconServiceProperty, value);
    }

    public static readonly StyledProperty<string> PathProperty =
        H.Property<string>()
            .OnChanged((e,a) =>
            {
                e.LoadIcon();
            })
            .Register();

    public static readonly StyledProperty<object> CaptionProperty =
        H.Property<object>()
            .OnChanged((s, a) => s.Update())
            .Register();

    public static readonly StyledProperty<double> IconMaxHeightProperty =
        H.Property<double>().Default(30.0).OnChanged((e,a) =>
            {
                if (double.IsNaN(e.IconMaxHeight)) return;
                e._iconElement.MaxHeight = e.IconMaxHeight;
            })
            .Register();

    public static readonly StyledProperty<double> IconMaxWidthProperty =
        H.Property<double>().Default(50.0).OnChanged((e,a) =>
            {
                if (double.IsNaN(e.IconMaxWidth)) return;
                e._iconElement.MaxWidth = e.IconMaxWidth;
            })
            .Register();


    public double MainBrush
    {
        get => GetValue(IconMaxWidthProperty);
        set => SetValue(IconMaxWidthProperty, value);
    }

    public string Path
    {
        get => (string)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    //TODO : Avalonia - [Content]    
    public object Caption
    {
        get => GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public double IconMaxHeight
    {
        get => GetValue(IconMaxHeightProperty);
        set => SetValue(IconMaxHeightProperty, value);
    }

    public double IconMaxWidth
    {
        get => GetValue(IconMaxWidthProperty);
        set => SetValue(IconMaxWidthProperty, value);
    }

    public IIconService? IconService
    {
        get => GetValue(IconServiceProperty);
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

    void LoadIcon()
    {
        var path = Path;

        if(string.IsNullOrEmpty(path)) return;

        if (IconService == null)
        {
#if DEBUG
            if (Design.IsDesignMode)
            {
                if (_designTimeService == null)
                {
                    _designTimeService = new IconService();
                    new IconBootloader(_designTimeService).LoadAsync(null);
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

        //Dispatcher.UIThread.InvokeAsync(
        //    async () =>
        //    {
        //        if (cancel.State) return;
        //        var icon = await iconService.GetIconTemplateAsync(path);
        //        if (cancel.State) return;
        //        if (icon is DataTemplate template)
        //        {
        //            _iconElement.ContentTemplate = template;
        //            Update();
        //        }
        //    }
        //    , XamlTools.Priority);



        Dispatcher.UIThread.InvokeAsync(
            async () =>
            {
                if (cancel.State) return;
                var icon = await iconService.GetIconAsync(path,Foreground?.ToColor().ToUInt32()??0);
                if (cancel.State) return;
                _iconElement.Content = icon;
                Update();
            }
            , XamlTools.Priority);

    }

    void Update()
    {
        if (!_attached) return;

        if (_iconElement.Content == null)
        {
            _iconElement.IsVisible = false;
            _spacer.Width = new GridLength(0.0);

            switch (Caption)
            {
                case null:
                case string c when string.IsNullOrWhiteSpace(c):
                    _captionElement.IsVisible = false;
                    break;
                default:
                    _captionElement.Content = Caption;
                    _captionElement.IsVisible = true;
                    break;
            }
        }
        else
        {
            _iconElement.IsVisible = true;
            switch (Caption)
            {
                case null:
                case string c when string.IsNullOrWhiteSpace(c):
                    _captionElement.IsVisible = false;
                    _spacer.Width = new GridLength(0.0);
                    break;
                default:
                    _captionElement.Content = Caption;
                    _captionElement.IsVisible = true;
                    _spacer.Width = new GridLength(10.0);
                    break;
            }
        }
    }

}