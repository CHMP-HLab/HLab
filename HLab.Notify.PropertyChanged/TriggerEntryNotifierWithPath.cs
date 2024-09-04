using System;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.PropertyChanged;

public class WeakTriggerEntryNotifierWithPath : WeakTriggerEntryNotifier
{
    readonly TriggerPath _path;
    ITriggerEntry _next;

    public WeakTriggerEntryNotifierWithPath(IPropertyEntry propertyEntry, TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler) 
        :base(propertyEntry,handler)
    {
        _path = path;
        propertyEntry.Link(OnPropertyChangedWithPath);
    }

    void OnPropertyChangedWithPath(object sender, ExtendedPropertyChangedEventArgs e)
    {
        _next?.Dispose();

        if(Handler.TryGetTarget(out var handler))
        {
            if (e.NewValue == null)
            {
                _next = null;
            }
            else
            {
                var nextParser = NotifyClassHelperBase.GetHelper(e.NewValue);
                _next = _path.GetTrigger(nextParser, handler);
            } 
        }
        else
        {
            _next = null;
        }
    }


    public override void Dispose()
    {
        base.Dispose();
        _next?.Dispose();

        PropertyEntry.Unlink(OnPropertyChangedWithPath);
    }

}
public class TriggerEntryNotifierWithPath : TriggerEntryNotifier
{
    readonly TriggerPath _path;
    ITriggerEntry _next;

    public TriggerEntryNotifierWithPath(IPropertyEntry propertyEntry, TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler) 
        :base(propertyEntry,handler)
    {
        _path = path;
        propertyEntry.Link(OnPropertyChangedWithPath);
    }

    void OnPropertyChangedWithPath(object sender, ExtendedPropertyChangedEventArgs e)
    {
        _next?.Dispose();

        if (e.NewValue == null)
        {
            _next = null;
        }
        else
        {
            var nextParser = NotifyClassHelperBase.GetHelper(e.NewValue);
            _next = _path.GetTrigger(nextParser, Handler);
        } 
    }


    public override void Dispose()
    {
        base.Dispose();
        _next?.Dispose();

        PropertyEntry.Unlink(OnPropertyChangedWithPath);
    }

}