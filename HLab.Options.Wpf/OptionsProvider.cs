using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using Microsoft.Win32;

namespace HLab.Options.Wpf
{
    [Export(typeof(IOptionsProvider))]
    public class OptionsProvider : IOptionsProvider
    {
        public string Name => "registry";

        private readonly IOptionsService _options;

        [Import] public OptionsProvider(IOptionsService options)
        {
            _options = options;
        }

        public Task SetValueAsync<T>(string name, T value, int? userid)
        {
            var t = new Task( () =>
                {
                    using var rk = Registry.CurrentUser.CreateSubKey(@"Software\" + _options.OptionsPath);
                    rk.SetValue(name, value?.ToString() ?? "");
                });
            t.Start();
            return t;
        }

        public Task<T> GetValueAsync<T>(string name, int? userid = null, Func<T> defaultValue = null)
        {
            var t = new Task<T>( () =>
            {
                using var rk = Registry.CurrentUser.OpenSubKey(@"Software\" + _options.OptionsPath);
                if (rk == null) return defaultValue==null?default:defaultValue();
                var s = rk.GetValue(name)?.ToString();

                return OptionsServices.GetValueFromString(s,defaultValue);
            });
            t.Start();
            return t;
        }
    }
}
