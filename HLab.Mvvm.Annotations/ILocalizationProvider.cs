using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations;

public interface ILocalizationProvider
{
    string Localize(string lang, string value);
    Task<string> LocalizeAsync(string lang, string value, CancellationToken token = default);
    ILocalizeEntry GetLocalizeEntry(string lang, string value);
    Task<ILocalizeEntry> GetLocalizeEntryAsync(string lang, string value);
    IEnumerable<ILocalizeEntry> GetLocalizeEntries(string value);
    IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string value, CancellationToken token = default);
    void Register(string tag, string code, string value, bool quality);
}