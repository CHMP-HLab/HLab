using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using Microsoft.Win32;

namespace HLab.Options.Wpf
{
    [Export(typeof(IOptionsProvider))]
    public class OptionsProvider : IOptionsProvider
    {
        [Import]
        private IOptionsService _options;

        public async Task SetValueAsync<T>(string name, T value, int? userid)
        {
            using (var rk = Registry.CurrentUser.CreateSubKey(@"Software\" + _options.OptionsPath))
            {
                rk.SetValue(name,value?.ToString()??"");
            }
        }

        public async Task<T> GetValueAsync<T>(string name, int? userid = null, Func<T> defaultValue = null)
        {
            using (var rk = Registry.CurrentUser.OpenSubKey(@"Software\" + _options.OptionsPath))
            {
                if (rk == null) return defaultValue==null?default:defaultValue();
                var s = rk.GetValue(name)?.ToString();

                return OptionsServices.GetValueFromString(s,defaultValue);
            }
        }
    }
}
