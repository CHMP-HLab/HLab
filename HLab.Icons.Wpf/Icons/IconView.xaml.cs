using System.ComponentModel;
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
            Update(IconService, Path, Caption,ForegroundMatchColor, BackgroundMatchColor);
        }

        public static readonly DependencyProperty PathProperty = 
            H.Property<string>().AffectsRender
            .OnChange((s, e) => s.Update(s.IconService, e.NewValue, s.Caption,s.ForegroundMatchColor, s.BackgroundMatchColor))
            .Register();

        public static readonly DependencyProperty CaptionProperty = 
            H.Property<object>().AffectsRender
            .OnChange((s, e) => s.Update(s.IconService, s.Path, e.NewValue, s.ForegroundMatchColor, s.BackgroundMatchColor))
            .Register();
 
        public static readonly DependencyProperty IconServiceProperty = 
            H.Property<IIconService>()
            .OnChange((s,e)=>
            {
                    s.Update(e.NewValue, s.Path, s.Caption,s.ForegroundMatchColor, s.BackgroundMatchColor);
            })
            .Inherits
            .RegisterAttached();

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

        public static readonly DependencyProperty ForegroundMatchColorProperty =
            H.Property<Color>()
                .Default(Colors.Black)
                .OnChange((s, e) => s.Update(s.IconService, s.Path, s.Caption,e.NewValue, s.BackgroundMatchColor))
                .Inherits.AffectsRender
                .RegisterAttached();

        public Color ForegroundMatchColor
        {
            get => (Color) GetValue(ForegroundMatchColorProperty);
            set => SetValue(ForegroundMatchColorProperty, value);
        }

        public static readonly DependencyProperty BackgroundMatchColorProperty =
            H.Property<Color>()
                .Default(Colors.White)
                .OnChange((s, e) => s.Update(s.IconService, s.Path, s.Caption, s.ForegroundMatchColor, e.NewValue))
                .Inherits.AffectsRender
                .RegisterAttached();
        public Color BackgroundMatchColor
        {
            get => (Color)GetValue(BackgroundMatchColorProperty);
            set => SetValue(BackgroundMatchColorProperty, value);
        }

        public IIconService IconService
        {
            get => (IIconService)GetValue(IconServiceProperty); 
            set => SetValue(IconServiceProperty, value);
        }

        private static IIconService _designTimeService = null;
        public async void Update(IIconService service, string path, object caption, Color foreMatch, Color backMatch)
        {
            if (!IsLoaded) return;
            if (service == null)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    if (_designTimeService == null)
                    {
                        _designTimeService = new IconService();
                        new IconBootloader(_designTimeService).Load(null);
                    }

                    service = _designTimeService;
                }
                else return;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                IconContent.Visibility = Visibility.Collapsed;
            }
            else
            {
                IconContent.Content = (UIElement) await service.GetIconAsync(path).ConfigureAwait(true);
                IconContent.Visibility = Visibility.Visible;
                #if DEBUG
                IconContent.ToolTip = path;
                #endif
            }

            switch (caption)
            {
                case null:
                case string c when string.IsNullOrWhiteSpace(c):
                    CaptionContent.Content = null;
                    CaptionContent.Visibility = Visibility.Collapsed;
                    break;
                default:
                    CaptionContent.Content = caption;
                    CaptionContent.Visibility = Visibility.Visible;
                    break;
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

        public static Color GetForegroundMatchColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(ForegroundMatchColorProperty);
        }
        public static void SetForegroundMatchColor(DependencyObject obj, Color value)
        {
            obj.SetValue(ForegroundMatchColorProperty, value);
        }

    }
}
