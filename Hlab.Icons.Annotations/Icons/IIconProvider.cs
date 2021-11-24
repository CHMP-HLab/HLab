using System.Drawing;
using System.Threading.Tasks;

namespace HLab.Icons.Annotations.Icons
{
    public interface IIconProvider
    {
        Task<object> GetAsync(object foreground, Size size = default);
        object Get(object foreground, Size size = default);
    }
}
