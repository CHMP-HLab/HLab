using System.IO;

namespace HLab.Core.Annotations
{
    public interface IOptionsService : IService
    {
        string GetOptionString(string name);
        StreamReader GetOptionFileReader(string name);
        StreamWriter GetOptionFileWriter(string name);
    }
}
