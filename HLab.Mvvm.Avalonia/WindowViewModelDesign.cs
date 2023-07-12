using HLab.Icons.Avalonia.Icons;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia;

public class WindowViewModelDesign
{
    public string Title => "Title";

    public IIconService IconService { get; }
    public ILocalizationService LocalizationService { get; }

}