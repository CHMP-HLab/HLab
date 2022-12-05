using System.Threading.Tasks;

namespace HLab.Icons.Annotations.Icons
{
    public interface IIconProvider
    {
        Task<object> GetAsync();
        object Get();
        Task<string> GetTemplateAsync();
    }
}
