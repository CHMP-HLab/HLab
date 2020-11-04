using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;

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

}
