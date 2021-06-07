using HLab.DependencyInjection;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;


namespace HLab.Notify
{
    public class NotifierModule //: Module
    {
        public /*override*/ void Load(DependencyInjectionContainer container)
        {
            container.Configure(c => c.ExportGenericAsTargetCovariant(typeof(INotifier<>), typeof(INotifierClass<>)).As<INotifierClass>());

            container.Configure(c => c.Export(typeof(Notifier<>)).GenericAsTarget().As<INotifier>());
        }
    }
}