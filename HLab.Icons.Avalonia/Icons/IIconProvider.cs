using Avalonia.Media;

namespace HLab.Icons.Avalonia.Icons
{
    public interface IIconProvider
    {
        object Get(IBrush? foreground);
        Task<object> GetAsync(IBrush? foreground);
        Task<string> GetTemplateAsync(IBrush? foreground);
    }
}
