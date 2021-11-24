using System.Drawing;
using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Icons.Annotations.Icons
{
    public interface IIconService : IService
    {
        Task<object> GetIconAsync(string path, object foreground = null, Size size = default);
        object GetIcon(string path, object foreground = null, Size size = default);

        void AddIconProvider(string name, IIconProvider provider);
        IIconProvider GetIconProvider(string name);
    }

}
