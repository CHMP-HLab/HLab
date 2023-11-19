using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations;

public interface ILocalizationService
{
    string  Localize(string lang, string value);
    string  Localize(string value);
    Task<string>  LocalizeAsync(string lang, string value, CancellationToken token = default);
    Task<string>  LocalizeAsync(string value, CancellationToken token = default);
    Task<ILocalizeEntry> GetLocalizeEntryAsync(string lang, string value, CancellationToken token = default);
    IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string value, CancellationToken token = default);
    void Set(CultureInfo info);
    void Register(ILocalizationProvider service);
}