using HLab.Base.Wpf;
using HLab.Icons.Annotations.Icons;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace HLab.Icons.Wpf.Icons
{
    using H = DependencyHelper<IconView>;

    [ContentProperty("Caption")]
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

            Loaded += IconView_Loaded;
        }

        static IconView()
        {
            ForegroundProperty.OverrideMetadata(typeof(IconView),new FrameworkPropertyMetadata(SystemColors.ControlTextBrush,
                            FrameworkPropertyMetadataOptions.Inherits,(s,e) => ((IconView)s).OnForegroundChanged(e)));

        }

        async void OnForegroundChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            if(eventArgs.NewValue != eventArgs.OldValue)
                await LoadIconAsync().ConfigureAwait(true);
        }

        async void IconView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadIconAsync().ConfigureAwait(true);
        }


        /// <summary>
        /// IconService
        /// </summary>
        public static readonly DependencyProperty IconServiceProperty =
            H.Property<IIconService>()
            .OnChange(async (e, a) =>
            {
                if (a.NewValue == null) return;
                if (e._iconServiceReference != null && e._iconServiceReference.TryGetTarget(out var iconService) && ReferenceEquals(a.NewValue, iconService)) return;

                e._iconServiceReference = new(a.NewValue);

                if (e.Path == null) return;

                await e.LoadIconAsync().ConfigureAwait(false);
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
            .OnChange(async (e, a) =>
            {
                if (a.NewValue == null) return;
                if (e.IsLoaded == false) return;

                await e.LoadIconAsync().ConfigureAwait(false);
            })
            .Register();

        public static readonly DependencyProperty CaptionProperty =
            H.Property<object>()
            .OnChange((s, e) => s.Update())
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
                e._iconElement.MaxWidth = a.NewValue;
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

        int _count = 0;

        async Task LoadIconAsync()
        {
            if(!IsLoaded) return;

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

            if (_count++>0) {}

            var icon = await IconService.GetIconAsync(Path,Foreground).ConfigureAwait(false);

            await Dispatcher.BeginInvoke(() =>
            {
                _iconElement.Content = icon;
                Update();
            });
        }

        void Update()
        {
            if (!IsLoaded) return;

            if (_iconElement.Content == null)
            {
                _iconElement.Visibility = Visibility.Collapsed;
                _spacer.Width = new GridLength(0.0);

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
                switch (Caption)
                {
                    case null:
                    case string c when string.IsNullOrWhiteSpace(c):
                        _captionElement.Visibility = Visibility.Collapsed;
                        _spacer.Width = new GridLength(0.0);
                        break;
                    default:
                        _captionElement.Content = Caption;
                        _captionElement.Visibility = Visibility.Visible;
                        _spacer.Width = new GridLength(10.0);
                        break;
                }
            }
        }

    }
}
