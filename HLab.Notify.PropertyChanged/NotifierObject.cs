using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using HLab.Base;
using HLab.DependencyInjection.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public abstract class NotifierObjectLegacy<T> : N<T>
    where T : NotifierObjectLegacy<T>
    {
        [Import]
        public Notifier N;

    }

    public class Boxed<T>
    {
        public T Value;
    }

    public abstract class NotifierTest<T> : NotifierBase
    where T : NotifierTest<T>
    {
        protected class H : NotifyHelper<T> { }

        private static readonly IEventHandlerService _eventHandlerService = new EventHandlerService();
        protected NotifierTest()
        {
            H.Initialize((T)this,OnPropertyChanged);
        }

    }

    public abstract class N<T> : NotifierBase
        //where T : N<T>
        where T : class
    {
        protected class H : NotifyHelper<T> { }
        protected N()
        {
            Initialize();
        }
        protected N(bool ignoreInitialize)
        {
        }

        protected void Initialize() => H.Initialize( this as T,OnPropertyChanged);
    }

}
