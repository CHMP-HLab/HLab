using Avalonia.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia;

public class WindowViewModelDesign : IDesignViewModel
{
    public WindowViewModelDesign()
    {
        if(!Design.IsDesignMode) throw new InvalidOperationException("Only for design mode");
    }
    public string Title => "Title";

    public IIconService IconService { get; }
    public ILocalizationService LocalizationService { get; }

}