using System;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;

namespace HLab.Mvvm.Application
{
    [Export(typeof(IApplicationInfoService)),Singleton]
    public class ApplicationInfoService : IApplicationInfoService
    {
        public Version Version { get; set; }
        public String Name { get; set; }
        public String DataSource { get; set; }
    }
}
