using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Application.Wpf
{
    using H = H<DocumentPresenter>;
    public class DocumentPresenter : ViewModel, IDocumentPresenter
    {
        readonly IMessagesService _message;
        readonly Func<object, ISelectedMessage> _getSelectedMessage;

        public DocumentPresenter
        (
            IMessagesService message,             
            Func<object, ISelectedMessage> getSelectedMessage 
        )
        {
            _message = message;
            _getSelectedMessage = getSelectedMessage;

            H.Initialize(this);
        }

        public ObservableCollection<object> Documents { get; } = new();
        public ObservableCollection<object> Anchorables { get; } = new();

        readonly List<object> _documentHistory = new();

        public object ActiveDocument
        {
            get => _activeDocument.Get();
            set
            {
                _documentHistory.Remove(value);
                _documentHistory.Insert(0, value);

                if (!_activeDocument.Set(value)) return;

                _message.Publish(_getSelectedMessage(value));
            }
        }
        readonly IProperty<object> _activeDocument = H.Property<object>();

        public bool RemoveDocument(object document)
        {
            if (!Documents.Contains(document)) return false;
            if (_documentHistory.Count <= 0 || !ReferenceEquals(_documentHistory[0], document)) return false;

            _documentHistory.Remove(document);
            if (_documentHistory.Count > 0)
            {
                ActiveDocument = _documentHistory[0];
            }
            Documents.Remove(document);

            return true;
        }

    }
}
