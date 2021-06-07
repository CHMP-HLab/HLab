using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using HLab.Base.Wpf;
using HLab.Icons.Annotations.Icons;

namespace HLab.Icons.Wpf.Icons
{
    using H = DependencyHelper<IconView>;
    /// <summary>
    /// Logique d'interaction pour Icon.xaml
    /// </summary>
    [ContentProperty("Caption")]
    public partial class IconView : UserControl
    {
        public IconView()
        {
            InitializeComponent();
            Loaded += IconView_Loaded;
        }

        private void IconView_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        public static readonly DependencyProperty IconServiceProperty =
            H.Property<IIconService>()
            .OnChange(async (s, e) =>
            {
                await s.UpdateIconServiceAsync(e.NewValue).ConfigureAwait(true);
            })
            .Inherits
            .RegisterAttached();

        public static readonly DependencyProperty PathProperty =
            H.Property<string>()
            .OnChange(async (s, e) => await s.UpdateIconAsync(e.NewValue).ConfigureAwait(true))
            .Register();

        public static readonly DependencyProperty CaptionProperty =
            H.Property<object>()
            .OnChange((s, e) => s.Update())
            .Register();

        public static readonly DependencyProperty IconMaxHeightProperty =
            H.Property<double>().Default(30.0)
            .Register();

        public static readonly DependencyProperty IconMaxWidthProperty =
            H.Property<double>().Default(50.0)
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

        //public static readonly DependencyProperty ForegroundMatchColorProperty =
        //    H.Property<Color>()
        //        .Default(Colors.Black)
        //        .OnChange((s, e) => s.Update())
        //        .Inherits
        //        .RegisterAttached();

        //public Color ForegroundMatchColor
        //{
        //    get => (Color)GetValue(ForegroundMatchColorProperty);
        //    set => SetValue(ForegroundMatchColorProperty, value);
        //}

        //public static readonly DependencyProperty BackgroundMatchColorProperty =
        //    H.Property<Color>()
        //        .Default(Colors.White)
        //        .OnChange((s, e) => s.Update())
        //        .Inherits
        //        .RegisterAttached();
        //public Color BackgroundMatchColor
        //{
        //    get => (Color)GetValue(BackgroundMatchColorProperty);
        //    set => SetValue(BackgroundMatchColorProperty, value);
        //}

        public IIconService IconService
        {
            get => (IIconService)GetValue(IconServiceProperty);
            set => SetValue(IconServiceProperty, value);
        }

        private static IIconService _designTimeService = null;

        public async Task UpdateIconAsync(string path)
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

            IconContent.Content = (UIElement)await IconService.GetIconAsync(path).ConfigureAwait(true);
            Update();
        }

        public async Task UpdateIconServiceAsync(IIconService service)
        {
            if (Path == null) return;
            await UpdateIconAsync(Path);
        }

        public void Update()
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

        //public static Color GetForegroundMatchColor(DependencyObject obj)
        //{
        //    return (Color)obj.GetValue(ForegroundMatchColorProperty);
        //}
        //public static void SetForegroundMatchColor(DependencyObject obj, Color value)
        //{
        //    obj.SetValue(ForegroundMatchColorProperty, value);
        //}

    }
}
