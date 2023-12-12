namespace HLab.Mvvm.Annotations;

public interface IMainViewModel
{
    string Title { get; }
    ILocalizationService LocalizationService { get; }
    public IIconService IconService { get; }
    public object MainIcon { get; }
}