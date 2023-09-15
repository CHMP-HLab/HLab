using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Mvvm.Annotations;

public interface IIconService : IService
{
    Task<object?> GetIconTemplateAsync(string path, uint foregroundColor = 0);
    Task<object?> GetIconAsync(string path, uint foregroundColor = 0);
    void AddIconProvider(string name, IIconProvider provider);
    IIconProvider GetIconProvider(string name);
}