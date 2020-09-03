using HLab.Notify.Annotations;
using System;
using System.ComponentModel;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged
{
    public class TriggerEntryNotifier : ITriggerEntry
    {
        public TriggerEntryNotifier(IPropertyEntry propertyEntry, TriggerPath path, ExtendedPropertyChangedEventHandler handler)
        {
            propertyEntry.ExtendedPropertyChanged += handler;

            _onDispose = () => propertyEntry.ExtendedPropertyChanged -= handler;

            if (path != null)
            {
                propertyEntry.Link(NextHandler);
                _onDispose = () => propertyEntry.Unlink(NextHandler);

                void NextHandler(object sender, PropertyChangedEventArgs arg)
                {
                    if (arg is ExtendedPropertyChangedEventArgs a)
                    {
                        _next?.Dispose();

                        if (a.NewValue == null)
                        {
                            _next = null;
                        }
                        else
                        {
                            var nextParser = NotifyClassHelper.GetHelper(a.NewValue);
                            _next = nextParser.GetTrigger(path, handler);
                        }

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
