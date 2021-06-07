using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Icons.Annotations.Icons;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Application.Wpf
{
    using H = H<MainWpfViewModel>;

    public class MainWpfViewModel : NotifierBase
    {
        public IAclService Acl {get; private set;}
        private IMessageBus _message;
        private IDocumentService _doc;
        public IApplicationInfoService ApplicationInfo { get; private set; }
        private Func<object, ISelectedMessage> _getSelectedMessage;
        public ILocalizationService LocalizationService { get; private set; }
        public IIconService IconService { get; private set; }

        public void Inject(
            IAclService acl, 
            IMessageBus message, 
            IDocumentService doc, 
            IApplicationInfoService applicationInfo, 
            ILocalizationService localizationService, 
            Func<object, ISelectedMessage> getSelectedMessage, 
            IIconService iconService)
        {
            Acl = acl;
            _message = message;
            doc.MainViewModel = this;
            _doc = doc;
            ApplicationInfo = applicationInfo;
            LocalizationService = localizationService;
            _getSelectedMessage = getSelectedMessage;
            IconService = iconService;

            H.Initialize(this);
        }


        public ObservableCollection<object> Anchorables { get; } = new();
        public ObservableCollection<object> Documents { get; } = new();



        public bool IsActive
        {
            get => _isActive.Get();
            set => _isActive.Set(value);
        }
        private readonly IProperty<bool> _isActive = H.Property<bool>(c => c.Default(true));


        public bool RemoveDocument(FrameworkElement document)
        {
            if (Documents.Contains(document))
            {
                if (_documentHistory.Count > 0 && ReferenceEquals(_documentHistory[0], document))
                {
                    _documentHistory.Remove(document);
                    if (_documentHistory.Count > 0)
                    {
                        ActiveDocument = _documentHistory[0];
                    }
                    Documents.Remove(document);
                }
            }

            return false;
        }

        private readonly List<FrameworkElement> _documentHistory = new List<FrameworkElement>();
        public FrameworkElement ActiveDocument
        {
            get => _activeDocument.Get();
            set
            {
                _documentHistory.Remove(value);
                _documentHistory.Insert(0,value);

                if (_activeDocument.Set(value))
                {
                    var message = _getSelectedMessage(value);
                    _message.Publish(message);
                }
            }
        }
        private readonly IProperty<FrameworkElement> _activeDocument = H.Property<FrameworkElement>();

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

        public Menu Menu { get; } = new Menu {IsMainMenu = true}; 

        public string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c.Set(e => e.ApplicationInfo.Name));

        public ICommand Exit  { get; } = H.Command(c => c
            .Action(e => System.Windows.Application.Current.Shutdown())
        );

        public ICommand OpenUserCommand { get; } = H.Command(c => c
           .Action(e => e._doc.OpenDocumentAsync(e.Acl.Connection.User))
        );


    }
}