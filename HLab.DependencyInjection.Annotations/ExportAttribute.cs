using System;

namespace HLab.DependencyInjection.Annotations
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class ExportAttribute : Attribute
    {
        public Func<IConfigurator,IConfigurator> Configurator { get; protected set; }

        protected ExportAttribute()
        { }

        public ExportAttribute(Func<IActivatorTree, bool> condition)
        {
            Configurator = c => c.When(condition);
        }

        public ExportAttribute(Type exportAs)
        {
            Configurator = c => c.As(exportAs);
        }
    }

    public class GenericAsTarget : ExportAttribute
    {
        public GenericAsTarget()
        {
            this.Configurator = c => c.GenericAsTarget();
        }
    }
}
