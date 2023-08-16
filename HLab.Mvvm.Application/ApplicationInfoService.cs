using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Xml.Schema;
using HLab.Core.ReactiveUI;
using HLab.Options;
using ReactiveUI;

namespace HLab.Mvvm.Application;

public class ApplicationInfoService : ReactiveObject, IApplicationInfoService
{
    readonly IOptionsService _options;

    public ApplicationInfoService(IOptionsService options )
    {
        _options = options;

        Themes.Add("{Dark}");
        Themes.Add("{Light}");
        Themes.Add("{Auto}");


        Theme = _options.GetValue("", "Theme", null, ()=>"", "registry");

        this.WhenAnyValue(e=> e.Theme).WhereNotNull().Subscribe(t => _options.SetValue("", "Theme", t, "registry"));
    }

    public Version Version { get; set; }
    public string Name { get; set; }
    public string DataSource { get; set; }

    public string Theme
    {
        get => _theme;
        set => this.RaiseAndSetIfChanged(ref _theme, value);
    }

    string _theme;

    public ObservableCollection<string> Themes { get; } = new();

}