using System;


namespace HLab.Notify.PropertyChanged;

public class WeakTriggerEntryNotifier : ITriggerEntry
{
    protected readonly IPropertyEntry PropertyEntry;
    protected readonly WeakReference<EventHandler<ExtendedPropertyChangedEventArgs>> Handler;

#if DEBUG
    readonly string _targetName;
#endif

    public WeakTriggerEntryNotifier(IPropertyEntry propertyEntry, EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
#if DEBUG
        _targetName = handler?.Target?.ToString();
#endif
        PropertyEntry = propertyEntry;
        if (handler == null) return;

        Handler = new(handler);
        propertyEntry.ExtendedPropertyChanged += OnPropertyChanged;
    }

    void OnPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
    {
        if (Handler.TryGetTarget(out var handler))
        {
            handler.Invoke(sender, e);
        }
        else
        { }
    }

    public virtual void Dispose()
    {
        PropertyEntry.ExtendedPropertyChanged -= OnPropertyChanged;
    }

    public override string ToString()
    {
        return $"{PropertyEntry.Name}";
    }
}
public class TriggerEntryNotifier : ITriggerEntry
{
    protected readonly IPropertyEntry PropertyEntry;
    protected readonly EventHandler<ExtendedPropertyChangedEventArgs> Handler;

#if DEBUG
    readonly string _targetName;
#endif

    public TriggerEntryNotifier(IPropertyEntry propertyEntry, EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
#if DEBUG
        _targetName = handler?.Target?.ToString();
#endif
        PropertyEntry = propertyEntry;
        if (handler == null) return;

        Handler = new(handler);
        propertyEntry.ExtendedPropertyChanged += OnPropertyChanged;
    }

    void OnPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
    {
        Handler.Invoke(sender, e);
    }

    public virtual void Dispose()
    {
        PropertyEntry.ExtendedPropertyChanged -= OnPropertyChanged;
    }

    public override string ToString()
    {
        return $"{PropertyEntry.Name}";
    }
}