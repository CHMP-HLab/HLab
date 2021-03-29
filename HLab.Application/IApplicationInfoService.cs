using System;

namespace HLab.Mvvm.Application
{
    public interface IApplicationInfoService
    {

        Version Version { get; set; }
        String Name { get; set; }
        String DataSource { get; set; }
    }
}
