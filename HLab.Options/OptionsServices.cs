using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;

namespace HLab.Options
{
    [Export(typeof(IOptionsService)), Singleton]
    public class OptionsServices : IOptionsService
    {
        public string OptionsPath { get; set; }
        public StreamReader GetOptionFileReader(string name)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), OptionsPath + @"\" + name);

            return new StreamReader(fileName);
        }

        public StreamWriter GetOptionFileWriter(string name)
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), OptionsPath + @"\" + name);

            var dir = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(dir);

            return new StreamWriter(fileName);
        }

        private readonly ConcurrentDictionary<string,IOptionsProvider> _providers = new ConcurrentDictionary<string, IOptionsProvider>();

        public void AddProvider(string name, IOptionsProvider provider) => _providers.AddOrUpdate(name, n => provider,(n,p) => provider);

        public void SetValue<T>(string name, T value, string provider = null, int? userid = null)
            => SetValueAsync(name, userid, provider, userid);


        public T GetValue<T>(string name, int? userid = null, Func<T> defaultValue = null, string providerName = null) 
            => GetValueAsync(name, defaultValue, providerName, userid).Result;

        public async Task<T> GetValueAsync<T>(string name, Func<T> defaultValue = null, string providerName = null, int? userid = null)
        {
            if (!string.IsNullOrWhiteSpace(providerName))
            {
                return await _providers[providerName].GetValueAsync<T>(name, userid);
            }

            foreach (var provider in _providers.Values)
            {
                var result = await provider.GetValueAsync<T>(name, userid);
                return result;
            }

            return defaultValue == null ? default : defaultValue();
        }

        public async Task SetValueAsync<T>(string name, T value, string providerName=null, int? userid=null)
        {
            if (providerName != null)
            {
                await _providers[providerName].SetValueAsync(name, value, userid);
            }
            foreach (var provider in _providers.Values)
            {
                await provider.SetValueAsync<T>(name, value, userid);
            }
        }
        public static T GetValueFromString<T>(string value, Func<T> defaultValue = null)
        {
            if (value == null)
            {
                if (defaultValue != null) return defaultValue();

                return default(T);
            }


            if (typeof(T) == typeof(string))
                return (T)(object)value;

            if (typeof(T) == typeof(double))
                if (double.TryParse(value, out var result))
                {
                    return (T)(object)result;
                }
            if (typeof(T) == typeof(int))
                if (int.TryParse(value, out var result))
                {
                    return (T)(object)result;
                }

            if (typeof(T).IsEnum)
                if (Enum.TryParse(typeof(T),value, out var result))
                {
                    return (T)result;
                }

            return default;
        }

        public ServiceState ServiceState { get; }
    }
}
