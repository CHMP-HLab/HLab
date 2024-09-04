using System;
using HLab.Notify.Annotations;

namespace HLab.Notify.Avalonia;

internal class ExtendedPropertyChangedEventEventManager //: WeakEventManager
{
    ExtendedPropertyChangedEventEventManager()
    {
    }

    /// <summary>
    /// Add a handler for the given source's event.
    /// </summary>
    public static void AddHandler(IPropertyEntry source,
        EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        // Todo : avalonia
        //CurrentManager.ProtectedAddHandler(source, handler);
    }

    /// <summary>
    /// Remove a handler for the given source's event.
    /// </summary>
    public static void RemoveHandler(IPropertyEntry source,
        EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        // Todo : avalonia
        //CurrentManager.ProtectedRemoveHandler(source, handler);
    }

    /// <summary>
    /// Get the event manager for the current thread.
    /// </summary>
    static ExtendedPropertyChangedEventEventManager CurrentManager
    {
        get
        {
            var managerType = typeof(ExtendedPropertyChangedEventEventManager);
            var manager = (ExtendedPropertyChangedEventEventManager)null;//GetCurrentManager(managerType);

            // at first use, create and register a new manager
            if (manager == null)
            {
                manager = new ExtendedPropertyChangedEventEventManager();
                //SetCurrentManager(managerType, manager);
            }

            return manager;
        }
    }

    /// <summary>
    /// Return a new list to hold listeners to the event.
    /// </summary>
    protected /*override ListenerList*/ void NewListenerList()
    {
        //return new ListenerList<ExtendedPropertyChangedEventArgs>();
    }

    /// <summary>
    /// Listen to the given source for the event.
    /// </summary>
    protected /*override*/ void StartListening(object source)
    {
        var typedSource = (IPropertyEntry)source;
        typedSource.ExtendedPropertyChanged += OnSomeEvent;
    }

    /// <summary>
    /// Stop listening to the given source for the event.
    /// </summary>
    protected /*override*/ void StopListening(object source)
    {
        var typedSource = (IPropertyEntry)source;
        typedSource.ExtendedPropertyChanged -= OnSomeEvent;
    }

    /// <summary>
    /// Event handler for the SomeEvent event.
    /// </summary>
    void OnSomeEvent(object sender, ExtendedPropertyChangedEventArgs e)
    {
        //DeliverEvent(sender, e);
    }
}