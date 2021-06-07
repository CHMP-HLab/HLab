using System;

namespace HLab.DependencyInjection.Annotations
{
    /// <summary>
    ///   Automate importation while Locate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field| AttributeTargets.Constructor |AttributeTargets.Parameter)]
    public class ImportAttribute : Attribute
    {
        public ImportAttribute(InjectLocation location = InjectLocation.BeforeConstructor)
        {
            Location = location;
        }

        public InjectLocation Location { get; }

    }
}