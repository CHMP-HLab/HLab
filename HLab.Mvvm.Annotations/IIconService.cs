using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Mvvm.Annotations
{
    public interface IIconService : IService
    {
        Task<object> GetIcon(string name, string foreMatch = "#FF000000", string backMatch = "#FFFFFFFF");
        object GetFromHtml(string html);
        Task<object> FromSvgString(string svg, string foreMatch = "#FF000000", string backMatch = "#FFFFFFFF");

        //object FromXaml(string xaml);
        void AddIconProvider(string name, IIconProvider provider);

        dynamic Icon { get; }
    }

}
