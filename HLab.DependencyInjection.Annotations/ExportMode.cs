using System;

namespace HLab.DependencyInjection.Annotations
{
    [Flags]
    public enum ExportMode
    {
        Default = 0,
        Singleton = 1,
        Decorator = 2,
    }
}
