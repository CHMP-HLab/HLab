using System.Threading.Tasks;

namespace HLab.Mvvm.Application
{
    public interface IDocumentService
    {
        Task OpenDocumentAsync(object content);
        Task CloseDocumentAsync(object content);

        object MainViewModel { get; set; }
    }
}
