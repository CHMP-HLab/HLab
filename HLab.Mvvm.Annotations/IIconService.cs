using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Mvvm.Annotations
{
    public interface IIconService : IService
    {
        Task<object> GetIconAsync(string path);
        object GetFromHtml(string html);
        Task<object> FromSvgStringAsync(string svg);

        //object FromXaml(string xaml);
        void AddIconProvider(string name, IIconProvider provider);

        dynamic Icon { get; }
    }

}
