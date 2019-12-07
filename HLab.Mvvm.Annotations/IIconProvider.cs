
using System.Drawing;
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations
{
    public interface IIconProvider
    {
        Task<object> GetAsync();
    }
}
