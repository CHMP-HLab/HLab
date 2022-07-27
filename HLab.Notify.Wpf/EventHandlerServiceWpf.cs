using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HLab.Notify.Annotations;

namespace HLab.Notify.Wpf
{
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

#if true
       public void AddHandler(IPropertyEntry source, EventHandler<ExtendedPropertyChangedEventArgs> handler) => ExtendedPropertyChangedEventEventManager.AddHandler(source,handler);

        public void RemoveHandler(IPropertyEntry source, EventHandler<ExtendedPropertyChangedEventArgs> handler) => ExtendedPropertyChangedEventEventManager.RemoveHandler(source,handler);
        #else
         public void AddHandler(IPropertyEntry source,
            EventHandler<ExtendedPropertyChangedEventArgs> handler)
            => source.ExtendedPropertyChanged += handler;

        public void RemoveHandler(IPropertyEntry source, EventHandler<ExtendedPropertyChangedEventArgs> handler)
            => source.ExtendedPropertyChanged -= handler;
#endif

        public void AddHandler(INotifyPropertyChanged source, EventHandler<PropertyChangedEventArgs> handler,string propertyName) => PropertyChangedEventManager.AddHandler(source, handler,propertyName);

        public void RemoveHandler(INotifyPropertyChanged source, EventHandler<PropertyChangedEventArgs> handler,string propertyName) => PropertyChangedEventManager.RemoveHandler(source,handler,propertyName);

        public void AddHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler) => CollectionChangedEventManager.AddHandler(source,handler);

        public void RemoveHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler) => CollectionChangedEventManager.RemoveHandler(source,handler);

        public void AddHandler<TSource,TArgs>(TSource source, string name, EventHandler<TArgs> handler) where TArgs : EventArgs
            => WeakEventManager<TSource,TArgs>.AddHandler(source,name,handler);
        public void RemoveHandler<TSource,TArgs>(TSource source, string name, EventHandler<TArgs> handler) where TArgs : EventArgs
            => WeakEventManager<TSource,TArgs>.RemoveHandler(source,name,handler);

        public IGuiTimer GetTimer() => new GuiTimer();
    }


    public class GuiTimer : IGuiTimer
    {
        readonly DispatcherTimer _timer;
        readonly Dispatcher _dispatcher;

        public GuiTimer()
        {
            _dispatcher = Application.Current.Dispatcher;
            _timer = new DispatcherTimer(DispatcherPriority.DataBind,_dispatcher);
            _timer.Tick += _timer_Tick;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            Tick?.Invoke(sender, e);
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
        public void DoTick()
        {
            _dispatcher.BeginInvoke(() => Tick?.Invoke(this, new EventArgs()));
            _timer.Start();
        }

        public TimeSpan Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public bool IsEnabled
        {
            get => _timer.IsEnabled; 
            set => _timer.IsEnabled = value;
        }

        public event EventHandler Tick;
    }
}
