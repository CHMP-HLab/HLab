﻿using System;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HLab.Mvvm
{
    [Export(typeof(ILocalizationService)),Singleton]
    public class LocalizationService : ILocalizationService, IService
    {
        private CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public Task<string> LocalizeAsync(string value) => LocalizeAsync(Culture.IetfLanguageTag.ToLowerInvariant(), value);

        private const StringComparison culture = StringComparison.InvariantCulture;

        public async Task<string> LocalizeItem(string tag, string code)
        {
            string result = code;
            bool quality = false;

            if (code.StartsWith("^"))
            {
                var codes = code.Split('|');
                code = codes[0].Substring(1);
                result = code;
                quality = false;
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
            }


            foreach (var service in _services)
            {
                var value = await service.LocalizeAsync(tag, code).ConfigureAwait(false);
                if (value != null) return value;
            }

            foreach (var service in _services)
            {
                service.Register(tag, code, result, quality);
            }

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

        public async Task<string> LocalizeAsync(string tag, string text)
        {
            tag = tag.ToLower();
            var matches = Regex.Matches(text, @"\{[^}]*}");
            foreach (Match match in matches)
            {
                var value = match.Value.Substring(1, match.Length - 2);
                var parts = value.Split("=");
                if (parts.Length > 1)
                {
                    var p = parts[0].ToLower();
                    if (p == tag || p == tag.Substring(0, 2))
                    {
                        text = text.Replace(match.Value, parts[1]);
                        continue;
                    }
                    text = text.Replace(match.Value, "");
                    continue;
                }

                var loc = await LocalizeItem(tag, value).ConfigureAwait(false);
                text = text.Replace(match.Value,loc);
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

    }

}
