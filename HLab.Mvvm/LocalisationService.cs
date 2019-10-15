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

namespace HLab.Mvvm
{
    [Export(typeof(ILocalizationService)),Singleton]
    public class LocalizationService : ILocalizationService, IService
    {
        private CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public string Localize(string value) => Localize(Culture.IetfLanguageTag.ToLower(), value);

        public string Localize(string tag, string code)
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
                    var pos = c.IndexOf('=');
                    if (pos < 0)
                    {
                        result = c;
                        quality = false;
                        break;
                    }

                    if (c.StartsWith(tag))
                    {
                        result = c.Substring(pos + 1);
                        quality = true;
                        break;
                    }
                }
            }


            foreach (var service in _services)
            {
                var value = service.Localize(tag, code);
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
    }

}
