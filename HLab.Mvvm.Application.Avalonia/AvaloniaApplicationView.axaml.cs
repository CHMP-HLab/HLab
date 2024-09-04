using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HLab.Erp.Acl.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Avalonia;
using HLab.Options;

namespace HLab.Mvvm.Application.Avalonia;

/// <summary>
/// Logique d'interaction pour MainWindow.xaml
/// </summary>
public partial class AvaloniaApplicationView : UserControl
    , IView<AvaloniaApplicationViewModel>
    , IView<ViewModeKiosk, AvaloniaApplicationViewModel>
{
    const string LayoutFileName = "layout.xml";

    public AvaloniaApplicationView(IOptionsService options/*, IDragDropService drag*/)
    {
        _options = options;
        InitializeComponent();

        //LoadLayout(); // Hang on printing forms

        Loaded += MainWindow_Loaded;
        DataContextChanged += OnDataContextChanged;
        //drag.RegisterDragCanvas(DragCanvas);
    }


    void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is not Window w) return;

        w.Closing += W_Closing;

        w.Position = new PixelPoint(
            _options.GetValue("Display","X",null,()=>0,"registry"),
            _options.GetValue("Display","Y",null,()=>0,"registry"));

        //SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight

        w.Width = _options.GetValue("Display","Width",null,()=>0.0,"registry");
        w.Height = _options.GetValue("Display","Height",null,()=>0.0,"registry");

        w.WindowState = _options.GetValue("Display","WindowState",null,()=>WindowState.Maximized,"registry");
    }

    void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is not Window w) return;

        _options.SetValue("Display","Y",w.Position.Y,"registry");
        _options.SetValue("Display","X",w.Position.X,"registry");
        _options.SetValue("Display","Width",w.Width,"registry");
        _options.SetValue("Display","Height",w.Height,"registry");

        _options.SetValue("Display","WindowState",w.WindowState,"registry");
    }

    readonly IOptionsService _options;

    void OnDataContextChanged(object? sender, EventArgs e)
    {
        var ctx = this.GetValue(ViewLocator.MvvmContextProperty);
    }


}