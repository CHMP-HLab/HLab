using System;

namespace HLab.Erp.Core
{
    public interface IApplicationInfoService
    {

        Version Version { get; set; }
        String Name { get; set; }
    }
}
