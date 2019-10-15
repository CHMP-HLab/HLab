using System;
using System.Collections.Generic;

namespace HLab.DependencyInjection.Annotations
{
    public interface IConfigurator
    {
        IExportLocatorScope Container { get; }
        List<IExportEntry> Entries { get; }
        IExportEntry LastEntry { get; }

        IConfigurator NewEntry();

    }

}
