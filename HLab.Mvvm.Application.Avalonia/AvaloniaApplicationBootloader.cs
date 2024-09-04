using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Avalonia;
using ReactiveUI;

namespace HLab.Mvvm.Application.Avalonia;

public class AvaloniaApplicationBootloader(ApplicationBootloader.Injector injector) : ApplicationBootloader(injector)
{
    // TODO : should not be needed anymore
    //public Window MainWindow { get; protected set; }

    public override void Load(IBootContext b)
    {
        base.Load(b);

        var window = injector.Mvvm.MainContext.GetView(ViewModel,MainViewMode,typeof(IDefaultViewClass))?.AsWindow();
        // TODO 
        //MainWindow.Closing += (sender, args) => System.Windows.Application.Current.Shutdown();
        window?.Show();
    }
}