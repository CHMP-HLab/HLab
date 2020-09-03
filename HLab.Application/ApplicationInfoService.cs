using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;

namespace HLab.Mvvm.Application
{
    [Export(typeof(IApplicationInfoService))]
    public class ApplicationInfoService : IApplicationInfoService
    {
        public Version Version { get; set; }
        public String Name { get; set; }
    }
}
