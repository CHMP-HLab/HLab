using System;

namespace HLab.Mvvm.Application
{
    public interface IApplicationInfoService
    {

        Version Version { get; set; }
        string Name { get; set; }
        string DataSource { get; set; }
    }
}
