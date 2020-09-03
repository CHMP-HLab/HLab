using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using HLab.Base;
using HLab.DependencyInjection.Annotations;

namespace HLab.Notify.PropertyChanged
{

    public class Boxed<T>
    {
        public T Value;
    }

    public abstract class NotifierTest<T> : NotifierBase
    where T : NotifierTest<T>
    {
        protected class H : H<T> { }

        private static readonly IEventHandlerService _eventHandlerService = new EventHandlerService();
        protected NotifierTest()
        {
            H<NotifierTest<T>>.Initialize(this);
            H.Initialize((T)this);
        }

    }

    public abstract class N<T> : NotifierBase
        where T : class, INotifyPropertyChanged
    {
        protected class H : H<T> { }
        protected N(bool initialize=true)
        {
            if(initialize) Initialize();
        }

        protected void Initialize() => H.Initialize( this as T);
    }

}
