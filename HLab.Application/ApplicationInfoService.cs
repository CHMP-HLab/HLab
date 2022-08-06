using System;
using System.Collections.ObjectModel;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Application;

using H = H<ApplicationInfoService>;
public class ApplicationInfoService : NotifierBase, IApplicationInfoService
{
    public ApplicationInfoService()
    {
        H.Initialize(this);
        Themes.Add("Sombre");
        Themes.Add("Clair");
        Themes.Add("Système");
    }

    public Version Version { get; set; }
    public string Name { get; set; }
    public string DataSource { get; set; }
    public string Theme { get => _theme.Get(); set => _theme.Set(value); }
    readonly IProperty<string> _theme = H.Property<string>();

    public ObservableCollection<string> Themes { get; } = new();

}