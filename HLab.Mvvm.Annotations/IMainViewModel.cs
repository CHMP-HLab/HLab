using HLab.Icons.Annotations;
using HLab.Icons.Annotations.Icons;

namespace HLab.Mvvm.Annotations
{
    public interface IMainViewModel
    {
        string Title { get; }
        IIconService IconService { get; }
        ILocalizationService LocalizationService { get; }

    }
}
