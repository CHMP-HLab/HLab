using HLab.Notify.Annotations;
using System;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public class TriggerEntryNotifier : ITriggerEntry
    {
        public TriggerEntryNotifier(IPropertyEntry propertyEntry, TriggerPath path, PropertyChangedEventHandler handler)
        {
            propertyEntry.PropertyChanged += handler;

            _onDispose = () => propertyEntry.PropertyChanged -= handler;

            if (path != null)
            {
                propertyEntry.Link(NextHandler);
                _onDispose = () => propertyEntry.Unlink(NextHandler);

                void NextHandler(object sender, RegisterValueEventArgs arg)
                {
                    _next?.Dispose();

                    if (arg.NewValue == null)
                    {
                        _next = null;
                    }
                    else
                    {
                        var nextParser = NotifyFactory.GetParser(arg.NewValue);
                        _next = nextParser?.GetTrigger(path, handler);
                    }
                }
            }
        }

        private ITriggerEntry _next;
        private readonly Action _onDispose;

        public void Dispose()
        {
            _next?.Dispose();
            _onDispose?.Invoke();
        }

    }
}
