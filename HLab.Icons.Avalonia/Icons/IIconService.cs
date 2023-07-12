using Avalonia.Media;
using HLab.Core.Annotations;

namespace HLab.Icons.Avalonia.Icons
{
    public interface IIconService : IService
    {
        Task<object> GetIconTemplateAsync(string path, IBrush? foreground);
        Task<object> GetIconAsync(string path, IBrush? foreground);
        void AddIconProvider(string name, IIconProvider provider);
        IIconProvider GetIconProvider(string name);
    }

}
