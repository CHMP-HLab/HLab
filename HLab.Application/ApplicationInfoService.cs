using System;

namespace HLab.Mvvm.Application
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public Version Version { get; set; }
        public String Name { get; set; }
        public String DataSource { get; set; }
    }
}
