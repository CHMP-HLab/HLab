using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Xml.Schema;
using HLab.Notify.PropertyChanged;
using HLab.Options;

namespace HLab.Mvvm.Application;

using H = H<ApplicationInfoService>;
public class ApplicationInfoService : NotifierBase, IApplicationInfoService
{
    readonly IOptionsService _options;

    public ApplicationInfoService(IOptionsService options )
    {
        _options = options;
        H.Initialize(this);
        Themes.Add("{Dark}");
        Themes.Add("{Light}");
        Themes.Add("{Auto}");

        Theme = _options.GetValue("", "Theme", null, ()=>"", "registry");
    }

    public Version Version { get; set; }
    public string Name { get; set; }
    public string DataSource { get; set; }

    public string Theme
    {
        get => _theme.Get();
        set
        {
            if(_theme.Set(value))
                _options.SetValue<string>("", "Theme", value, "registry");
        }
    }

    readonly IProperty<string> _theme = H.Property<string>();

    public ObservableCollection<string> Themes { get; } = new();

}