using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Application.Wpf
{
    using H = H<MainWpfViewModel>;

    [Export(typeof(MainWpfViewModel)), Singleton]
    public class MainWpfViewModel : NotifierBase
    {

        [Import] public MainWpfViewModel(IAclService acl, IMessageBus message)
        {
            Acl = acl;
            _message = message;
            H.Initialize(this);
        }

       
        public IAclService Acl {get; }
        private readonly IMessageBus _message;

        [Import(InjectLocation.AfterConstructor)]
        public void SetDoc(IDocumentService doc)
        {
            doc.MainViewModel = this;
            _doc = doc;
        }

        private IDocumentService _doc;

        [Import]
        public IApplicationInfoService ApplicationInfo { get; }

        [Import]
        private readonly Func<object, SelectedMessage> _getSelectedMessage;

        [Import]
        public ILocalizationService LocalizationService { get; }
        [Import]
        public IIconService IconService { get; }

        public ObservableCollection<object> Anchorables { get; } = new ObservableCollection<object>();
        public ObservableCollection<object> Documents { get; } = new ObservableCollection<object>();



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