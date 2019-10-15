using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HLab.Base;
using HLab.DependencyInjection.Annotations;


namespace HLab.Notify.Wpf
{
    [Export(typeof(IEventHandlerService)),Singleton]
    public class EventHandlerServiceWpf : IEventHandlerService
    {
        public void Invoke(Delegate eventHandler, object source, EventArgs args)
        {
            if (eventHandler == null) return;
            var delegates = eventHandler.GetInvocationList();

            async void BeginInvoke(Delegate d)
            {
                //Debug.Print(args.ToString());
                    switch (d.Target)
                    {
                        case DispatcherObject dispatcherObject:
                            await dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);
                            break;

                        case ISynchronizeInvoke s:
                            s.BeginInvoke(d, new object[] { source, args });
                            break;

                        default:

                             Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);
                        
                            //d.Method.Invoke(d.Target, new object[] { source, args });
                            //d.DynamicInvoke(source, args); // note : this does not execute handler in target thread's context
                            break;
                   }
            }

            foreach(var d in delegates) BeginInvoke(d);

            //Parallel.ForEach(delegates, BeginInvoke);
        }

        public void Invoke(PropertyChangedEventHandler eventHandler, object source, PropertyChangedEventArgs args)
        {
            if (eventHandler == null) return;
            var delegates = eventHandler.GetInvocationList();

            async void BeginInvoke(Delegate d)
            {
                //Debug.Print(args.ToString());
                switch (d.Target)
                {
                    case DispatcherObject dispatcherObject:
                        await dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);
                        break;

                    case ISynchronizeInvoke s:
                        s.BeginInvoke(d, new[] { source, args });
                        break;

                    default:

                        Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);

                        //d.Method.Invoke(d.Target, new object[] { source, args });
                        //d.DynamicInvoke(source, args); // note : this does not execute handler in target thread's context
                        break;
                }
            }

            foreach (var d in delegates) BeginInvoke(d);
        }

        public void Invoke(NotifyCollectionChangedEventHandler eventHandler, object source, NotifyCollectionChangedEventArgs args)
        {
            if (eventHandler == null) return;
            var delegates = eventHandler.GetInvocationList();

            async void BeginInvoke(Delegate d)
            {
                //Debug.Print(args.ToString());
                switch (d.Target)
                {
                    case DispatcherObject dispatcherObject:
                        await dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);
                        break;

                    case ISynchronizeInvoke s:
                        s.BeginInvoke(d, new[] { source, args });
                        break;

                    default:

                        Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);

                        //d.Method.Invoke(d.Target, new object[] { source, args });
                        //d.DynamicInvoke(source, args); // note : this does not execute handler in target thread's context
                        break;
                }
            }

            foreach (var d in delegates) BeginInvoke(d);
        }

        public void Invoke(EventHandler eventHandler, object source, EventArgs args)
        {
            if (eventHandler == null) return;
            var delegates = eventHandler.GetInvocationList();

            async void BeginInvoke(Delegate d)
            {
                //Debug.Print(args.ToString());
                switch (d.Target)
                {
                    case DispatcherObject dispatcherObject:
                        await dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);
                        break;

                    case ISynchronizeInvoke s:
                        s.BeginInvoke(d, new object[] { source, args });
                        break;

                    default:

                        Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);

                        //d.Method.Invoke(d.Target, new object[] { source, args });
                        //d.DynamicInvoke(source, args); // note : this does not execute handler in target thread's context
                        break;
                }
            }

            foreach (var d in delegates) BeginInvoke(d);
        }
    }
}
