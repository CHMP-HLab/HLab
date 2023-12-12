using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace HLab.Notify.Annotations;

public abstract class TriggerOnEntry : IDisposable
{
    protected readonly PropertyChangedEventHandler Handler;
    protected readonly TriggerPath Path;
    protected TriggerOnEntry(TriggerPath path, PropertyChangedEventHandler handler)
    {
        Path = path;
        Handler = handler;
    }

    public int Count = 1;

    protected TriggerOnEntry Subscribe(object value)
    {
        var path = Path;
        while (path != null)
        {
            var next = path.Next;

            if (value is INotifyCollectionChanged col && Path.PropertyName == "Item")
            {
                return new TriggerOnEntryCollection(Handler, col, next);
            }

            if (value is INotifierObject n)
            {
                return new TriggerOnEntryNotifier(Handler, n.GetNotifier().GetPropertyEntry(path.PropertyName), next);
            }

            var property = value.GetType().GetProperty(path.PropertyName);
            Debug.Assert(property != null,"Property \"" + path.PropertyName + "\" not found on " + value.GetType().Name);
            value = property.GetValue(value);
            path = next;
        }
        return null;
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            //resource?.Dispose();
        }
    }
}