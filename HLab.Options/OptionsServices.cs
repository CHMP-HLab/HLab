using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HLab.Core.Annotations;

namespace HLab.Options;

public class OptionsServices : IOptionsService
{
    readonly List<IOptionsProvider> _providers = new();

    public void AddProvider(IOptionsProvider provider)
    {
        provider.Options = this;
        _providers.Add(provider);
    }

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

    public IEnumerable<string> GetSubList(string path, string name, int? userid, string providerName = null)
        => GetSubListAsync(path, name, userid, providerName).Result;

    public IEnumerable<string> GetOptions(string path, string name, int? userid, string providerName = null)
        => GetOptionsAsync(path, name, userid, providerName).Result;

    public void SetValue<T>(string path, string name, T value, string provider = null, int? userid = null)
        => SetValueAsync(path, name, value, provider, userid);


    public T GetValue<T>(string path, string name, int? userid = null, Func<T> defaultValue = null, string providerName = null) 
        => GetValueAsync(path, name, defaultValue, providerName, userid).Result;

    public async Task<IEnumerable<string>> GetSubListAsync(string path, string name, int? userid, string providerName = null)
    {
        foreach (var provider in _providers)
        {
            if (!string.IsNullOrWhiteSpace(providerName) && providerName != provider.Name) continue;
            var result = await provider.GetSubListAsync(path,name, userid).ConfigureAwait(false);
            return result;
        }

        return new List<string>();
    }
    public async Task<IEnumerable<string>> GetOptionsAsync(string path, string name, int? userid, string providerName = null)
    {
        foreach (var provider in _providers)
        {
            if (!string.IsNullOrWhiteSpace(providerName) && providerName != provider.Name) continue;
            var result = await provider.GetOptionsAsync(path,name, userid).ConfigureAwait(false);
            return result;
        }

        return new List<string>();
    }

    public async Task<T> GetValueAsync<T>(string path, string name, Func<T> defaultValue = null, string providerName = null, int? userid = null)
    {
        foreach (var provider in _providers)
        {
            if (!string.IsNullOrWhiteSpace(providerName) && providerName != provider.Name) continue;
            var result = await provider.GetValueAsync<T>(path,name, userid).ConfigureAwait(false);
            return result;
        }

        return defaultValue == null ? default : defaultValue();
    }

    public async Task SetValueAsync<T>(string path, string name, T value, string providerName=null, int? userid=null)
    {
        foreach (var provider in _providers)
        {
            if (!string.IsNullOrWhiteSpace(providerName) && providerName != provider.Name) continue;
            await provider.SetValueAsync<T>(path, name, value, userid);
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