using System.Threading.Tasks;

namespace HLab.Mvvm.Application
{
    public interface IDocumentService
    {
        Task OpenDocumentAsync(object content);
        Task OpenDocumentAsync<T>();
        Task CloseDocumentAsync(object content);

        object MainViewModel { get; set; }
    }
}
