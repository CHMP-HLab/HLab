using System;
using System.ComponentModel;

namespace HLab.Notify.Annotations;

public class TriggerOnEntryNotifier : TriggerOnEntry
{
    public TriggerOnEntryNotifier(PropertyChangedEventHandler handler, INotifierPropertyEntry entry, TriggerPath path)
        : base(path,handler)
    {
        entry.PropertyChanged += Handler;
        _onDispose = () => entry.PropertyChanged -= Handler;

        if (Path == null) return;

        void NextHandler(object sender, PropertyChangedEventArgs arg)
        {
            _next?.Dispose();
            if (arg is NotifierPropertyChangedEventArgs notifierArg)
            {
                _next = notifierArg.NewValue != null ? Subscribe(notifierArg.NewValue) : null;
            }
        }


        entry.RegisterValue += NextHandler;
        _onDispose += () => entry.RegisterValue -= NextHandler;
        _onDispose += () => _next?.Dispose();

        if (!entry.IsSet)
            entry.Update();

        var value = entry.GetValue();

        NextHandler(null, new NotifierPropertyChangedEventArgs(entry, null, value));

    }

    TriggerOnEntry _next;
    readonly Action _onDispose;

    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;

        _next?.Dispose();
        _onDispose?.Invoke();
    }

}