using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace HLab.Options.Wpf
{
    public class OptionsProviderRegistry : IOptionsProvider
    {
        public string Name => "registry";

        public IOptionsService Options { get; set; }

        public Task<IEnumerable<string>> GetSubListAsync(string path, string name, int? userid)
        {
            var t = new Task<IEnumerable<string>>( () =>
                {
                    using var rk = Registry.CurrentUser.OpenSubKey(@$"Software\{Options.OptionsPath}{getPath(path)}\{name}");
                    return rk?.GetSubKeyNames().ToList() ?? new List<string>();
                });
            t.Start();
            return t;
        }
        public Task<IEnumerable<string>> GetOptionsAsync(string path, string name, int? userid)
        {
            var t = new Task<IEnumerable<string>>( () =>
                {
                    using var rk = Registry.CurrentUser.OpenSubKey(@$"Software\{Options.OptionsPath}{getPath(path)}\{name}");
                    return rk?.GetValueNames().ToList() ?? new List<string>();
                });
            t.Start();
            return t;
        }

        string getPath(string path) => (string.IsNullOrEmpty(path))?"":$@"\{path}";

        public Task SetValueAsync<T>(string path, string name, T value, int? userid)
        {
            var t = new Task( () =>
            {
                    using var rk = Registry.CurrentUser.CreateSubKey(@$"Software\{Options.OptionsPath}{getPath(path)}");
                    rk?.SetValue(name, value?.ToString() ?? "");
                });
            t.Start();
            return t;
        }

        public async Task<T> GetValueAsync<T>(string path, string name, int? userid = null, Func<T> defaultValue = null)
        {
            return await Task.Run( () =>
            {
                using var rk = Registry.CurrentUser.OpenSubKey(@$"Software\{Options.OptionsPath}{getPath(path)}");
                if (rk == null) return defaultValue==null?default:defaultValue();
                var s = rk.GetValue(name)?.ToString();

                return OptionsServices.GetValueFromString(s,defaultValue);
            }).ConfigureAwait(false);
        }
    }
}
