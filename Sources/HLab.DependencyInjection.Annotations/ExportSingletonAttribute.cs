using System;

namespace HLab.DependencyInjection.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : ExportAttribute
    {
        public SingletonAttribute()
        {
            Configurator = c => c.Singleton();
        }
    }
   [AttributeUsage(AttributeTargets.Class)]
    public class DecoratorAttribute : ExportAttribute
    {
        public DecoratorAttribute()
        {
            Configurator = c => c.Decorator();
        }
    }

}
