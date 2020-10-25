using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using Microsoft.Win32;

namespace HLab.Options.Wpf
{
    [Export(typeof(IOptionsProvider)),Singleton]
    public class OptionsProvider : IOptionsProvider
    {
        public string Name => "registry";

        public IOptionsService Options { get; set; }


        public Task SetValueAsync<T>(string name, T value, int? userid)
        {
            var t = new Task( () =>
                {
                    using var rk = Registry.CurrentUser.CreateSubKey(@"Software\" + Options.OptionsPath);
                    rk.SetValue(name, value?.ToString() ?? "");
                });
            t.Start();
            return t;
        }

        public async Task<T> GetValueAsync<T>(string name, int? userid = null, Func<T> defaultValue = null)
        {
            return await Task.Run( () =>
            {
                using var rk = Registry.CurrentUser.OpenSubKey(@"Software\" + Options.OptionsPath);
                if (rk == null) return defaultValue==null?default:defaultValue();
                var s = rk.GetValue(name)?.ToString();

                return OptionsServices.GetValueFromString(s,defaultValue);
            }).ConfigureAwait(false);
        }
    }
}
