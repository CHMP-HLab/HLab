using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Mvvm.Annotations;
using H = HLab.Base.DependencyHelper<HLab.Mvvm.Icons.IconView>;
namespace HLab.Mvvm.Icons
{
    /// <summary>
    /// Logique d'interaction pour Icon.xaml
    /// </summary>
    public partial class IconView : UserControl
    {
        public IconView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty = 
            H.Property<string>()
            .OnChange((s, e) => s.Update(s.IconService, e.NewValue, s.ForegroundMatchColor, s.BackgroundMatchColor))
            .AffectsRender
            .Register();
 
        public static readonly DependencyProperty IconServiceProperty = 
            H.Property<IIconService>()
            .OnChange((s,e)=>
            {
                    s.Update(e.NewValue, s.Path, s.ForegroundMatchColor, s.BackgroundMatchColor);
            })
            .Inherits.AffectsRender
            .RegisterAttached();

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly DependencyProperty ForegroundMatchColorProperty =
            H.Property<Color>()
                .Default(Colors.Black)
                .OnChange((s, e) => s.Update(s.IconService, s.Path, e.NewValue, s.BackgroundMatchColor))
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
                .OnChange((s, e) => s.Update(s.IconService, s.Path, s.ForegroundMatchColor, e.NewValue))
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
        public async void Update(IIconService service, string name, Color foreMatch, Color backMatch)
        {
            if (service == null)
            {
                if (_designTimeService == null)
                {
                    _designTimeService = new IconService();
                    new IconBootloader(_designTimeService).Load();
                }

                service = _designTimeService;
                //Content = new Canvas()
                //{
                //    Background = new SolidColorBrush(Color.FromScRgb(0.5f,1.0f,1.0f,1.0f)),
                //    Width = 32,
                //    Height = 32,
                //};
                //return;
            }
            if (string.IsNullOrWhiteSpace(name)) return;
            Content = (UIElement) await service.GetIconAsync(name).ConfigureAwait(true);
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
