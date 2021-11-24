using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using HLab.Base.Wpf;
using HLab.Icons.Annotations.Icons;

using Color = System.Windows.Media.Color;

namespace HLab.Icons.Wpf.Icons
{
    using H = DependencyHelper<IconView2>;
    /// <summary>
    /// Logique d'interaction pour Icon.xaml
    /// </summary>
    [ContentProperty("Caption")]
    public partial class IconView2 : UserControl
    {
        public IconView2()
        {
            InitializeComponent();
            Loaded += IconView_Loaded;
        }

        private WeakReference<IIconService> _iconServiceReference;
        private bool _iconLoaded = false;

        private async void IconView_Loaded(object sender, RoutedEventArgs e)
        {
            if(!_iconLoaded)
                await LoadIconAsync(Path).ConfigureAwait(true);
            
            Update();
        }


        public static readonly DependencyProperty IconServiceProperty =
            H.Property<IIconService>()
            .OnChange(async (e, a) =>
            {
                if(a.NewValue ==  null) return;
                if(e._iconServiceReference!=null && e._iconServiceReference.TryGetTarget(out var iconService) && ReferenceEquals(a.NewValue,iconService)) return;

                e._iconServiceReference = new(a.NewValue);

                if (e.Path == null) return;
                e._iconLoaded = false;

                if(e.IsLoaded == false) return;

                await e.LoadIconAsync(e.Path).ConfigureAwait(false);
            })
            .Inherits
            .RegisterAttached();

        public static readonly DependencyProperty PathProperty =
            H.Property<string>()
            .OnChange(async (e, a) =>
            {
                if (a.NewValue == null) return;
                e._iconLoaded = false;

                if(e.IsLoaded == false) return;

                await e.LoadIconAsync(a.NewValue).ConfigureAwait(false);
            })
            .Register();

        public static readonly DependencyProperty CaptionProperty =
            H.Property<object>()
            .OnChange((s, e) => s.Update())
            .Register();

        public static readonly DependencyProperty IconMaxHeightProperty =
            H.Property<double>().Default(30.0).OnChange((e, a) =>
            {
                if(double.IsNaN(a.NewValue)) return;
                e.IconContent.MaxHeight = a.NewValue;
            })
            .Register();

        public static readonly DependencyProperty IconMaxWidthProperty =
            H.Property<double>().Default(50.0).OnChange((e, a) =>
            {
                if(double.IsNaN(a.NewValue)) return;
                e.IconContent.MaxWidth = a.NewValue;
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

        private static IIconService _designTimeService = null;


        int count = 0;
        private async Task LoadIconAsync(string path)
        {
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

            if(count++>0) return;

            var icon = await IconService.GetIconAsync(path).ConfigureAwait(false);

            await Dispatcher.BeginInvoke(() =>
            {
                IconContent.Content = icon;
                Update();
                _iconLoaded = true;
            });
        }


        private void Update()
        {
            if(!IsLoaded) return;

            if (IconContent.Content == null)
            {
                IconContent.Visibility = Visibility.Collapsed;
                Spacer.Width = new GridLength(0.0);

                switch (Caption)
                {
                    case null:
                    case string c when string.IsNullOrWhiteSpace(c):
                        CaptionContent.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        CaptionContent.Content = Caption;
                        CaptionContent.Visibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                IconContent.Visibility = Visibility.Visible;
                switch (Caption)
                {
                    case null:
                    case string c when string.IsNullOrWhiteSpace(c):
                        CaptionContent.Visibility = Visibility.Collapsed;
                        Spacer.Width = new GridLength(0.0);
                        break;
                    default:
                        CaptionContent.Content = Caption;
                        CaptionContent.Visibility = Visibility.Visible;
                        Spacer.Width = new GridLength(10.0);
                        break;
                }
            }
        }

        private string StringColor(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static IIconService GetIconService(DependencyObject obj)
        {
            return (IIconService)obj.GetValue(IconServiceProperty);
        }
        public static void SetIconService(DependencyObject obj, IIconService value)
        {
            obj.SetValue(IconServiceProperty, value);
        }

    }
}
