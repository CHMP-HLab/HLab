#define DONT_USE_WEAK_HANDLERS

using System;
using System.Collections.Concurrent;
using NotifyClassHelper = HLab.Notify.PropertyChanged.NotifyHelpers.NotifyClassHelper;

namespace HLab.Notify.PropertyChanged;

internal class TriggerEntryCollectionWithPath : TriggerEntryCollection
{
    readonly TriggerPath _path;

    public TriggerEntryCollectionWithPath(IPropertyEntry propertyEntry, TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        : base(propertyEntry, handler)
    {
        _path = path;
        PropertyEntry.Link(OnPropertyChangedWithPath);
    }

    void OnPropertyChangedWithPath(object sender, ExtendedPropertyChangedEventArgs e)
    {
        if (e.OldValue != null && _next.TryRemove(e.OldValue, out var oldTrigger))
        {
            oldTrigger.Dispose();
        }
        if (e.NewValue != null)
        {
            var handler = Handler;

            _next.GetOrAdd(e.NewValue,
                o => _path.GetTrigger(NotifyClassHelper.GetHelper(o),handler));
        }
    }


    readonly ConcurrentDictionary<object, ITriggerEntry> _next = new ConcurrentDictionary<object, ITriggerEntry>();
    public override void Dispose()
    {
        PropertyEntry.Unlink(OnPropertyChangedWithPath);
        foreach (var triggerEntry in _next.Values)
        {
            triggerEntry.Dispose();
        }
    }
}