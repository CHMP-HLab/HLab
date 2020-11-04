using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Notify.Wpf
{
    [Export(typeof(IEventHandlerService)), Singleton]
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
                        try
                        {
                            d.DynamicInvoke(source, args);
                        }
                        catch
                        {
                            Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);
                        }
                        //d.Method.Invoke(d.Target, new object[] { source, args });
                        //d.DynamicInvoke(source, args); // note : this does not execute handler in target thread's context
                        break;
                }
            }

            foreach (var d in delegates) BeginInvoke(d);

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
                        d.Method.Invoke(d.Target, new object[] {source, args});
                        //Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, d, source, args);
                        //var t = new Task(() => d.Method.Invoke(d.Target, new object[] {source, args}));
                        //t.Start();
                        //await t;
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


        public async Task InvokeAsync(Func<Task> action)
        {
            await Application.Current.Dispatcher.InvokeAsync(action);
        }
        public void Invoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        public void AddHandler(IPropertyEntry source, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            ExtendedPropertyChangedEventEventManager.AddHandler(source,handler);
        }

        public void RemoveHandler(IPropertyEntry source, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            ExtendedPropertyChangedEventEventManager.RemoveHandler(source,handler);
        }

    }
}
