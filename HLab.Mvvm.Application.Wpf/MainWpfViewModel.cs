using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Icons.Annotations.Icons;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Application.Wpf
{
    using H = H<MainWpfViewModel>;


    public class MainWpfViewModelDesign : MainWpfViewModel
    {
        public MainWpfViewModelDesign() 
            : base(null, null, null, null, null, null )
        {
            
        }
    }

    public class MainWpfViewModel : NotifierBase
    {
        public IAclService Acl {get; }
        readonly IDocumentService _doc;
        public IApplicationInfoService ApplicationInfo { get; }
        public ILocalizationService LocalizationService { get; }
        public IIconService IconService { get; }

        public MainWpfViewModel(
            IAclService acl, 
            IDocumentService doc, 
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

            H.Initialize(this);
        }

        public IDocumentPresenter DocumentPresenter { get; }

        public bool IsActive
        {
            get => _isActive.Get();
            set => _isActive.Set(value);
        }
        readonly IProperty<bool> _isActive = H.Property<bool>(c => c.Default(true));




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

        public Menu Menu { get; } = new Menu {IsMainMenu = true, Background=Brushes.Transparent}; 

        public string Title => _title.Get();
        readonly IProperty<string> _title = H.Property<string>(c => c.Set(e => e.ApplicationInfo.Name));

        public ICommand Exit  { get; } = H.Command(c => c
            .Action(e => System.Windows.Application.Current.Shutdown())
        );

        public ICommand OpenUserCommand { get; } = H.Command(c => c
           .Action(e => e._doc.OpenDocumentAsync(e.Acl.Connection.User))
        );


    }
}