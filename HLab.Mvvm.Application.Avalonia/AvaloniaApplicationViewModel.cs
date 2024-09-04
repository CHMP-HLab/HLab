using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Icons.Avalonia.Icons;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Menus;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Mvvm.Application.Avalonia;

public class AvaloniaApplicationViewModel : ViewModel, IApplicationViewModel
{
    public IAclService Acl {get; }
    readonly IDocumentService _doc;
    public IApplicationInfoService ApplicationInfo { get; }
    public ILocalizationService LocalizationService { get; }
    public IIconService IconService { get; }

    public AvaloniaApplicationViewModel(
        IAclService acl, 
        IDocumentService doc, 
        IMenuService menu, 
        IDocumentPresenter presenter, 
        IApplicationInfoService applicationInfo, 
        ILocalizationService localizationService, 
        IIconService iconService)
    {
        Acl = acl;
        DocumentPresenter = presenter;
        doc.MainPresenter = presenter;
        _doc = doc;
        ApplicationInfo = applicationInfo;
        LocalizationService = localizationService;
        IconService = iconService;

        Menu = menu.MainMenu;

        _title = this
            .WhenAnyValue(
                e => e.ApplicationInfo.Name,
                e => e.Acl.Connection.User,
                (a, u) => $"{a} - {u?.Name}"
            )
            .ToProperty(this, e => e.Title);

        Exit = ReactiveCommand.Create(() =>
        {
            // TODO
            //System.Windows.Application.Current.Shutdown();
        });

        OpenUserCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _doc.OpenDocumentAsync(Acl.Connection.User);
        });
    }

    public IDocumentPresenter DocumentPresenter { get; }

    public bool IsActive
    {
        get => _isActive;
        set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }
    bool _isActive = true;


    // TODO
    //public Canvas DragCanvas => _dragCanvas.Get();
    //private readonly IProperty<Canvas> _dragCanvas = H.Property<Canvas>( c => c
    //    .Set( e => {
    //            var canvas = new Canvas();
    //            e._dragDrop.RegisterDragCanvas(canvas);
    //            return canvas;
    //        }
    //    )
    //);

    public object Menu { get; }  //IsMainMenu = true, 

    public string Title => _title.Value;
    readonly ObservableAsPropertyHelper<string> _title;

    public ICommand Exit  { get; } 

    public ICommand OpenUserCommand { get; }


}