using System.Drawing;
using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Icons.Annotations.Icons
{
    public interface IIconService : IService
    {
        Task<object> GetIconAsync(string path);

        Task<object> GetIconBitmapAsync(string name, Size size);

        //object FromXaml(string xaml);
        void AddIconProvider(string name, IIconProvider provider);
    }

}
