using System;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;

namespace HLab.Mvvm;

public partial class LocalizationService : ILocalizationService, IService
{
    CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

    public string Localize(string value) => Localize(Culture.IetfLanguageTag.ToLowerInvariant(), value);
    public Task<string> LocalizeAsync(string value, CancellationToken token = default) 
        => LocalizeAsync(Culture.IetfLanguageTag.ToLowerInvariant(), value, token);

    public ILocalizeEntry GetLocalizeEntry(string language, string code)
    {
        foreach (var service in _services)
        {
            var value = service.GetLocalizeEntry(language, code);
            if(value != null ) return value;
        }

        return null;
    }
    public async Task<ILocalizeEntry> GetLocalizeEntryAsync(string language, string code, CancellationToken token)
    {
        foreach (var service in _services)
        {
            if (token.IsCancellationRequested) return default;
            var value = await service.GetLocalizeEntryAsync(language, code).ConfigureAwait(false);
            if(value != null ) return value;
        }

        return null;
    }
    public IEnumerable<ILocalizeEntry> GetLocalizeEntries(string code)
    {
        foreach (var service in _services)
        {
            foreach(var value in service.GetLocalizeEntries(code))
                yield return value;
        }
    }

    public async IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string code, CancellationToken token = default)
    {
        foreach (var service in _services)
        {
            await foreach(var value in service.GetLocalizeEntriesAsync(code,token).ConfigureAwait(false))
                yield return value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag">language tag : en,fr,.. </param>
    /// <param name="code">"^en=yes|fr=oui"</param>
    /// <param name="quality"></param>
    /// <returns></returns>
    static (string,bool) SelfLocalized(string tag, string code)
    {
        var quality = false;

        if (!code.StartsWith("^")) return (code,quality);

        var codes = code.Split('|');
        code = codes[0][1..];
        var result = code;
        foreach (var c in codes.Skip(1))
        {
            var pos = c.IndexOf('=',StringComparison.InvariantCulture);
            if (pos < 0)
            {
                result = c;
                quality = false;
                break;
            }

            if (!c.StartsWith(tag, StringComparison.InvariantCulture)) continue;

            result = c[(pos + 1)..];
            quality = true;
            break;
        }
        return (result,quality);
    }

    void Register(string tag, string code, string result, bool quality)
    {
        foreach (var service in _services)
        {
            service.Register(tag, code, result, quality);
        }
    }

    public string LocalizeItem(string tag, string code)
    {
        var (result,quality) = SelfLocalized(tag, code);

        foreach (var service in _services)
        {
            var value = service.Localize(tag, code);
            if (value != null) return value;
        }

        Register(tag, code, result, quality);

        return result;
    }

    public async Task<string> LocalizeItemAsync(string tag, string code, CancellationToken token)
    {
        foreach (var service in _services)
        {
            var value = await service.LocalizeAsync(tag, code, token).ConfigureAwait(false);
            if (value != null) return value;
        }

        var (result,quality) = SelfLocalized(tag, code);

        Register(tag, code, result, quality);

        return result;
    }

    readonly List<ILocalizationProvider> _services = new List<ILocalizationProvider>();

    public void Set(CultureInfo info)
    {
        throw new NotImplementedException();
    }

    public void Register(ILocalizationProvider service)
    {
        _services.Add(service);
    }

    OneOf<string,NotFound> SelfLocalize(string text, string replaceValue, string tag, string value)
    {
        var parts = value.Split("=");
        if (parts.Length > 1)
        {
            var p = parts[0].ToLower().Replace("us","en");
            if (p == tag || p == tag.Substring(0, 2))
            {
                return text.Replace(replaceValue, parts[1]);
            }
            return text.Replace(replaceValue, "");
        }

        return default(NotFound);
    }



    public string Localize(string tag, string text)
    {   
        if (string.IsNullOrWhiteSpace(text)) return "";
        tag = tag.ToLower();
        var matches = InsideBrackets().Matches(text);
        foreach (Match match in matches)
        {
            var value = match.Groups[1].Value;
            var bracketValue = match.Value;


            text = SelfLocalize(text, bracketValue, tag, value).Match(
                t => t,
                _ => text.Replace(bracketValue,LocalizeItem(tag, value))
            );
        }

        return text;
    }

    public async Task<string> LocalizeAsync(string tag, string text, CancellationToken token=default)
    {   
        if (string.IsNullOrWhiteSpace(text)) return "";
        tag = tag.ToLower();
        var matches = InsideBrackets().Matches(text);
        foreach (Match match in matches)
        {
            var value = match.Groups[1].Value;
            var bracketValue = match.Value;

            text = await SelfLocalize(text, bracketValue, tag, value).Match<Task<string>>(
                Task.FromResult,
                async _ => text = text.Replace(bracketValue,await LocalizeItemAsync(tag, value,token))
                    
            );
        }

        return text;
    }


    public static string KeepLanguage(string text, string lang)
    {
        return Regex.Replace(text, @"\{" + lang + @"=([\s|!-\|~-■]*)}", "$1");
    }

    public static string CleanUpLanguage(string text)
    {
        return Regex.Replace(text,@"\{..=[\s|!-\|~-■]*}", "");
    }

    public ServiceState ServiceState { get; internal set; } = ServiceState.Available;

    [GeneratedRegex("\\{([^}]*)}")]
    private static partial Regex InsideBrackets();


}