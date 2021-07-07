using System;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HLab.Mvvm
{
    public class LocalizationService : ILocalizationService, IService
    {
        private CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public string Localize(string value) => Localize(Culture.IetfLanguageTag.ToLowerInvariant(), value);
        public Task<string> LocalizeAsync(string value) => LocalizeAsync(Culture.IetfLanguageTag.ToLowerInvariant(), value);
        public ILocalizeEntry GetLocalizeEntry(string language, string code)
        {
            foreach (var service in _services)
            {
                var value = service.GetLocalizeEntry(language, code);
                if(value != null ) return value;
            }

            return null;
        }
        public async Task<ILocalizeEntry> GetLocalizeEntryAsync(string language, string code)
        {
            foreach (var service in _services)
            {
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

        public async IAsyncEnumerable<ILocalizeEntry> GetLocalizeEntriesAsync(string code)
        {
            foreach (var service in _services)
            {
                await foreach(var value in service.GetLocalizeEntriesAsync(code).ConfigureAwait(false))
                    yield return value;
            }
        }



        private string SelfLocalized(string tag, string code, out bool quality)
        {
            quality = false;

            if (!code.StartsWith("^")) return code;

            var codes = code.Split('|');
            code = codes[0].Substring(1);
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

                if (c.StartsWith(tag,StringComparison.InvariantCulture))
                {
                    result = c.Substring(pos + 1);
                    quality = true;
                    break;
                }
            }
            return result;
        }

        private void Register(string tag, string code, string result, bool quality)
        {
            foreach (var service in _services)
            {
                service.Register(tag, code, result, quality);
            }
        }

        public string LocalizeItem(string tag, string code)
        {
            var result = SelfLocalized(tag, code, out var quality);

            foreach (var service in _services)
            {
                var value = service.Localize(tag, code);
                if (value != null) return value;
            }

            Register(tag, code, result, quality);

            return result;
        }

        public async Task<string> LocalizeItemAsync(string tag, string code)
        {
            var result = SelfLocalized(tag, code, out var quality);

            foreach (var service in _services)
            {
                var value = await service.LocalizeAsync(tag, code).ConfigureAwait(false);
                if (value != null) return value;
            }

            Register(tag, code, result, quality);

            return result;
        }

        private readonly List<ILocalizationProvider> _services = new List<ILocalizationProvider>();

        public void Set(CultureInfo info)
        {
            throw new System.NotImplementedException();
        }

        public void Register(ILocalizationProvider service)
        {
            _services.Add(service);
        }

        private string SelfLocalize(string text, string replaceValue, string tag, string value, out bool done)
        {
                var parts = value.Split("=");
                if (parts.Length > 1)
                {
                    done = true;
                    var p = parts[0].ToLower().Replace("us","en");
                    if (p == tag || p == tag.Substring(0, 2))
                    {
                         return text.Replace(replaceValue, parts[1]);
                    }
                    return text.Replace(replaceValue, "");
                }

                done = false;
                return text;
        }



        public string Localize(string tag, string text)
        {   
            if (string.IsNullOrWhiteSpace(text)) return "";
            tag = tag.ToLower();
            var matches = Regex.Matches(text, @"\{([^}]*)}");
            foreach (Match match in matches)
            {
                var value = match.Groups[1].Value;
                var bracketValue = match.Value;

                text = SelfLocalize(text, bracketValue, tag, value, out var done);
                if (done) continue;

                text = text.Replace(bracketValue,LocalizeItem(tag, value));
            }

            return text;
        }

        public async Task<string> LocalizeAsync(string tag, string text)
        {   
            if (string.IsNullOrWhiteSpace(text)) return "";
            tag = tag.ToLower();
            var matches = Regex.Matches(text, @"\{([^}]*)}");
            foreach (Match match in matches)
            {
                var value = match.Groups[1].Value;
                var bracketValue = match.Value;

                text = SelfLocalize(text, bracketValue, tag, value, out var done);
                if (done) continue;

                text = text.Replace(bracketValue,await LocalizeItemAsync(tag, value));
            }

            return text;
        }


        public static String KeepLanguage(String text, String lang)
        {
            return Regex.Replace(text, @"\{" + lang + @"=([\s|!-\|~-■]*)}", "$1");
        }

        public static string CleanUpLanguage(string text)
        {
            return Regex.Replace(text,@"\{..=[\s|!-\|~-■]*}", "");
        }

        public ServiceState ServiceState { get; internal set; } = ServiceState.Available;
    }

}
