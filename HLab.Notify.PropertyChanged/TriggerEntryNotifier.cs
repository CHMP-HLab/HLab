using HLab.Notify.Annotations;
using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged
{

    public class TriggerEntryNotifier : ITriggerEntry
    {
        public TriggerEntryNotifier(IPropertyEntry propertyEntry, TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            

            NotifyHelper.EventHandlerService.AddHandler(propertyEntry,handler); //propertyEntry.ExtendedPropertyChanged += handler;

            _onDispose = () =>  NotifyHelper.EventHandlerService.RemoveHandler(propertyEntry,handler); //propertyEntry.ExtendedPropertyChanged -= handler;

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
