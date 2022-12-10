using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using Avalonia.Threading;
using HLab.Base.Avalonia;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Avalonia.Icons;

using H = DependencyHelper<IconView>;


public class IconView : ContentControl
{
    WeakReference<IIconService> _iconServiceReference;
    static IIconService _designTimeService = null;

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
    }

    static IconView()
    {
        /* TODO
        ForegroundProperty.OverrideMetadata<IconView>(typeof(IconView), new FrameworkPropertyMetadata(SystemColors.ControlTextBrush,
                        FrameworkPropertyMetadataOptions.Inherits, (s, e) => ((IconView)s).OnForegroundChanged(e)));
        */
    }

    void OnForegroundChanged(StyledPropertyChangedEventArgs<IconView> eventArgs)
    {
        if (eventArgs.NewValue != eventArgs.OldValue)
            LoadIcon(Path);
    }

    bool _attached = false;
    void IconView_Loaded(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
    {
        _attached = true;
        LoadIcon(Path);
    }


    /// <summary>
    /// IconService
    /// </summary>
    public static readonly StyledProperty<IIconService> IconServiceProperty =
        H.Property<IIconService>()
            .OnChange(async (e, a) =>
            {
                //if (a.NewValue == null) return;
                //if (e._iconServiceReference != null && e._iconServiceReference.TryGetTarget(out var iconService) && ReferenceEquals(a.NewValue, iconService)) return;

                //e._iconServiceReference = new(a.NewValue);

                //if (e.Path == null) return;

                //await e.LoadIconAsync().ConfigureAwait(false);
            })
            .Inherits
            .RegisterAttached();

    public static IIconService GetIconService(StyledElement obj)
    {
        return (IIconService)obj.GetValue(IconServiceProperty);
    }

    public static void SetIconService(StyledElement obj, IIconService value)
    {
        obj.SetValue(IconServiceProperty, value);
    }

    public static readonly StyledProperty<string> PathProperty =
        H.Property<string>()
            .OnChangeBeforeNotification(e =>
            {
                // TODO avalonia : Loaded
                e.LoadIcon(e.Path);
            })
            .Register();

    public static readonly StyledProperty<object> CaptionProperty =
        H.Property<object>()
            .OnChange((s, e) => s.Update())
            .Register();

    public static readonly StyledProperty<double> IconMaxHeightProperty =
        H.Property<double>().Default(30.0).OnChangeBeforeNotification(e =>
            {
                if (double.IsNaN(e.IconMaxHeight)) return;
                e._iconElement.MaxHeight = e.IconMaxHeight;
            })
            .Register();

    public static readonly StyledProperty<double> IconMaxWidthProperty =
        H.Property<double>().Default(50.0).OnChangeBeforeNotification(e =>
            {
                if (double.IsNaN(e.IconMaxWidth)) return;
                e._iconElement.MaxWidth = e.IconMaxWidth;
            })
            .Register();


    public double MainBrush
    {
        get => (double)GetValue(IconMaxWidthProperty);
        set => SetValue(IconMaxWidthProperty, value);
    }

    public string Path
    {
        get => (string)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    [Content]    
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

    int _count = 0;
    readonly ConcurrentStack<Canceler> _cancel = new();

    void LoadIcon(string path)
    {
        if(path==null) return;
        if (IconService == null)
        {
#if DEBUG
            if (Design.IsDesignMode)
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

        var t2 = Dispatcher.UIThread.InvokeAsync(
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