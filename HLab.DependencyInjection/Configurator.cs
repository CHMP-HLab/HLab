using System;
using System.Collections.Generic;
using System.Linq;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    class Configurator : IConfigurator
    {
        public Configurator(IExportLocatorScope container)
        {
            Container = container;
        }

        public IExportLocatorScope Container { get; }
        public List<IExportEntry> Entries { get; } = new List<IExportEntry>();
        public IExportEntry LastEntry { get; private set; }
        public IConfigurator NewEntry()
        {
            LastEntry = new ExportEntry(Container);
            Entries.Add(LastEntry);
            return this;
        }
    }
}
