using System.Collections.Generic;
using System.Globalization;

namespace HLab.Mvvm.Annotations
{
    public interface ILocalizationService
    {
        string Localize(string lang, string value);
        string Localize(string value);
        void Set(CultureInfo info);
        void Register(ILocalizationProvider service);
    }

    public interface ILocalizationProvider
    {
        string Localize(string lang, string value);
        void Register(string tag, string code, string value, bool quality);
    }
}
