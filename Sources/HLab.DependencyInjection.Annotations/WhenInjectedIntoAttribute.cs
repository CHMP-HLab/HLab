using System;

namespace HLab.DependencyInjection.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WhenInjectedIntoAttribute : ExportAttribute
    {
        public WhenInjectedIntoAttribute(Type type)
        {
            Configurator = c => c.When(t => type.IsAssignableFrom(t.Context.TargetType));
        }
    }
}