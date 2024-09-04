using System.Collections.ObjectModel;

namespace HLab.Mvvm.Application.Documents;

public interface IDocumentPresenter
{
    ObservableCollection<object> Documents { get; }
    ObservableCollection<object> Anchorables { get; }
    object ActiveDocument { get; set; }
    bool RemoveDocument(object document);
}