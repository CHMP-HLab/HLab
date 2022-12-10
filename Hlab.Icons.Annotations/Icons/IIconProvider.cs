using System.Threading.Tasks;

namespace HLab.Icons.Annotations.Icons
{
    public interface IIconProvider
    {
        object Get();
        Task<object> GetAsync();
        Task<string> GetTemplateAsync();
    }
}
