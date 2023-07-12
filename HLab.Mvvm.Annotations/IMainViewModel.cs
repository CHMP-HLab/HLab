using System;

namespace HLab.Mvvm.Annotations;

public interface IMainViewModel
{
    string Title { get; }
    ILocalizationService LocalizationService { get; }
}