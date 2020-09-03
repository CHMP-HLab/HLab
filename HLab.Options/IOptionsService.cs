using HLab.Core.Annotations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HLab.Options
{
    public interface IOptionsService : IService
    {
        string OptionsPath { get; set; }

        StreamReader GetOptionFileReader(string name);
        StreamWriter GetOptionFileWriter(string name);

        void SetValue<T>(string name, T value, string providerName = null, int? userid = null);
        T GetValue<T>(string name, int? userid = null, Func<T> defaultValue = null, string providerName = null);

        Task<T> GetValueAsync<T>(string name, Func<T> defaultValue = null, string providerName = null,
            int? userid = null);
        Task SetValueAsync<T>(string name, T value, string providerName = null, int? userid = null);
    }

    public interface IOptionsProvider
    {
        Task<T> GetValueAsync<T>(string name,int? userid = null, Func<T> defaultValue = null);
        Task SetValueAsync<T>(string name, T value, int? userid = null);
    }
}
