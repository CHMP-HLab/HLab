using System.Threading.Tasks;

namespace HLab.Mvvm.Application.Documents;

public interface IDocumentService
{
    Task OpenDocumentAsync(object content, IDocumentPresenter presenter);
    Task CloseDocumentAsync(object content, IDocumentPresenter presenter);

    Task OpenDocumentAsync(object content) => OpenDocumentAsync(content, MainPresenter);
    Task OpenDocumentAsync<T>(IDocumentPresenter presenter) => OpenDocumentAsync(typeof(T), presenter);
    Task OpenDocumentAsync<T>() => OpenDocumentAsync<T>(MainPresenter);
    Task CloseDocumentAsync(object content) => CloseDocumentAsync(content, MainPresenter);

    IDocumentPresenter MainPresenter { get; set; }
}
