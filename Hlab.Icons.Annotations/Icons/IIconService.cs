using System.Drawing;
using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Icons.Annotations.Icons
{
    public interface IIconService : IService
    {
        Task<object> GetIconTemplateAsync(string path);
        void AddIconProvider(string name, IIconProvider provider);
        IIconProvider GetIconProvider(string name);
    }

}
