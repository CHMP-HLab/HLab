
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations
{
    public interface IIconProvider
    {
        Task<object> Get(string backMatch, string foreMatch);
    }
}
