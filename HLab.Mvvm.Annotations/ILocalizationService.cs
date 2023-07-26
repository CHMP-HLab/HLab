using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations;

public class LocalizeEntryDesign : ILocalizeEntry
{
    public LocalizeEntryDesign(string code)
    {
        Tag = string.Empty;
        Code = code;
        Value = code;
    }

    public string Tag { get; set; }
    public string Code { get; set; }
    public string Value { get; set; }
}

public interface ILocalizeEntry
{
    string Tag { get; set; }
    string Code { get; set; }
    string Value { get; set; }
}

public class LocalizationServiceDesign : ILocalizationService
{
    public string Localize(string lang, string value) => value;
    public string Localize(string value) => value; 
    public Task<string> LocalizeAsync(string lang, string value) => new(()=>value);
    public Task<string> LocalizeAsync(string value) => new(()=>value);

    public Task<ILocalizeEntry> GetLocalizeEntryAsync(string lang, string value) => new(()=>new LocalizeEntryDesign(value));

    public async IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string value)
    {
        yield return new LocalizeEntryDesign(value);
    }

    public void Set(CultureInfo info){}
    public void Register(ILocalizationProvider service){}
}

public interface ILocalizationService
{
    string  Localize(string lang, string value);
    string  Localize(string value);
    Task<string>  LocalizeAsync(string lang, string value);
    Task<string>  LocalizeAsync(string value);
    Task<ILocalizeEntry> GetLocalizeEntryAsync(string lang, string value);
    IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string value);
    void Set(CultureInfo info);
    void Register(ILocalizationProvider service);
}

public interface ILocalizationProvider
{
    string Localize(string lang, string value);
    Task<string> LocalizeAsync(string lang, string value);
    ILocalizeEntry GetLocalizeEntry(string lang, string value);
    Task<ILocalizeEntry> GetLocalizeEntryAsync(string lang, string value);
    IEnumerable<ILocalizeEntry> GetLocalizeEntries(string value);
    IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string value);
    void Register(string tag, string code, string value, bool quality);
}
