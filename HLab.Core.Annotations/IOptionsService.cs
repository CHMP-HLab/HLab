using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HLab.Core.Annotations
{
    public interface IOptionsService : IService
    {
        string GetOptionString(string name);
        StreamReader GetOptionFileReader(string name);
        StreamWriter GetOptionFileWriter(string name);

        Task<T> GetValue<T>(string name,int? userid, Func<T> defaultValue = null);
        void SetValue<T>(string name, T value, int? userid);
        Task SetValueAsync<T>(string name, T value, int? userid);

        public void SetDataService(IService data);
    }
}
