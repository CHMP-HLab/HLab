using System.Drawing;
using System.Threading.Tasks;

namespace HLab.Icons.Annotations.Icons
{
    public interface IIconProvider
    {
        Task<object> GetAsync(Size size);
        object Get(Size size = default);
    }
}
