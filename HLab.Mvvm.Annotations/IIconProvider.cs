using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations;

public interface IIconProvider
{
    object Get(uint foregroundColor = 0);
    Task<object> GetAsync(uint foregroundColor = 0);
    Task<string> GetTemplateAsync(uint foregroundColor = 0);
}