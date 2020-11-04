using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HLab.Notify.Annotations
{



    public interface IEventHandlerService
    {
        void Invoke(PropertyChangedEventHandler eventHandler, object source, PropertyChangedEventArgs args);
        void Invoke(NotifyCollectionChangedEventHandler eventHandler, object source, NotifyCollectionChangedEventArgs args);
        void Invoke(EventHandler eventHandler, object source, EventArgs args);
        Task InvokeAsync(Func<Task> action);
        void Invoke(Action action);

        public void AddHandler(IPropertyEntry source,
            EventHandler<ExtendedPropertyChangedEventArgs> handler);
        public void RemoveHandler(IPropertyEntry source,
            EventHandler<ExtendedPropertyChangedEventArgs> handler);
    }

    public class EventHandlerService : IEventHandlerService
    {
        public void Invoke(PropertyChangedEventHandler eventHandler, object source, PropertyChangedEventArgs args)
        {
            eventHandler?.Invoke(source,args);
        }

        public void Invoke(NotifyCollectionChangedEventHandler eventHandler, object source, NotifyCollectionChangedEventArgs args)
        {
            eventHandler?.Invoke(source,args);
        }

        public void Invoke(EventHandler eventHandler, object source, EventArgs args)
        {
            eventHandler?.Invoke(source, args);
        }

        public Task InvokeAsync(Func<Task> action)
        {
            return action();
        }
        public void Invoke(Action action)
        {
            action();
        }

        public void AddHandler(IPropertyEntry source,
            EventHandler<ExtendedPropertyChangedEventArgs> handler)
            => source.ExtendedPropertyChanged += handler;

        public void RemoveHandler(IPropertyEntry source, EventHandler<ExtendedPropertyChangedEventArgs> handler)
            => source.ExtendedPropertyChanged -= handler;

    }
}