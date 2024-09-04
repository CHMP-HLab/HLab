using HLab.Core.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HLab.Options;

public interface IOptionsService : IService
{
    string OptionsPath { get; set; }

    StreamReader GetOptionFileReader(string name);
    StreamWriter GetOptionFileWriter(string name);

    IEnumerable<string> GetSubList(string path, string name, int? userid, string providerName = null);
    IEnumerable<string> GetOptions(string path, string name, int? userid, string providerName = null);


    void SetValue<T>(string path, string name, T value, string providerName = null, int? userid = null);
    T GetValue<T>(string path, string name, int? userid = null, Func<T> defaultValue = null, string providerName = null);

    Task<IEnumerable<string>> GetSubListAsync(string path, string name, int? userid, string providerName = null);
    Task<IEnumerable<string>> GetOptionsAsync(string path, string name, int? userid, string providerName = null);


    Task<T> GetValueAsync<T>(string path, string name, Func<T> defaultValue = null, string providerName = null,
        int? userid = null);
    Task SetValueAsync<T>(string path, string name, T value, string providerName = null, int? userid = null);
    void AddProvider(IOptionsProvider provider);
}

public interface IOptionsProvider
{
    string Name { get; }
    IOptionsService Options { get; set; }
    Task<IEnumerable<string>> GetSubListAsync(string path, string name, int? userid);
    Task<IEnumerable<string>> GetOptionsAsync(string path, string name, int? userid);

    Task<T> GetValueAsync<T>(string path, string name,int? userid = null, Func<T> defaultValue = null);
    Task SetValueAsync<T>(string path, string name, T value, int? userid = null);
}