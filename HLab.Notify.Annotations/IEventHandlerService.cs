using HLab.Core.Annotations;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HLab.Notify.Annotations;

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

    public void AddHandler(INotifyPropertyChanged source, EventHandler<PropertyChangedEventArgs> handler,string propertyName) ;
    public void RemoveHandler(INotifyPropertyChanged source, EventHandler<PropertyChangedEventArgs> handler,string propertyName);

    public void AddHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler);
    public void RemoveHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler);

    void AddHandler<TSource, TArgs>(TSource source, string name, EventHandler<TArgs> handler)
        where TArgs : EventArgs;

    void RemoveHandler<TSource, TArgs>(TSource source, string name, EventHandler<TArgs> handler)
        where TArgs : EventArgs;

    IGuiTimer GetTimer();
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


    public void AddHandler(INotifyPropertyChanged source, EventHandler<PropertyChangedEventArgs> handler,string propertyName)
    {
        throw new NotImplementedException();
    }

        
    public void RemoveHandler(INotifyPropertyChanged source, EventHandler<PropertyChangedEventArgs> handler,string propertyName)
    {
        throw new NotImplementedException();
    }

    public void AddHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler)
    {
        throw new NotImplementedException();
    }

    public void RemoveHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler)
    {
        throw new NotImplementedException();
    }

    public void AddHandler<TSource, TArgs>(TSource source, string name, EventHandler<TArgs> handler) where TArgs : EventArgs
    {
        throw new NotImplementedException();
    }

    public void RemoveHandler<TSource, TArgs>(TSource source, string name, EventHandler<TArgs> handler) where TArgs : EventArgs
    {
        throw new NotImplementedException();
    }

    public IGuiTimer GetTimer()
    {
        throw new NotImplementedException();
    }
}