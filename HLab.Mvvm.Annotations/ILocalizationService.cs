using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations
{
    public interface ILocalizationService
    {
        Task<string>  LocalizeAsync(string lang, string value);
        Task<string>  LocalizeAsync(string value);
        void Set(CultureInfo info);
        void Register(ILocalizationProvider service);
    }

    public interface ILocalizationProvider
    {
        Task<string> LocalizeAsync(string lang, string value);
        void Register(string tag, string code, string value, bool quality);
    }
}
