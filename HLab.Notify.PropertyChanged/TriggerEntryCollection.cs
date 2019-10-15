using HLab.Notify.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HLab.Notify.PropertyChanged
{
    public static class CollectionExtension
    {
        public static T Item<T>(this IEnumerable<T> col)
        {
            return default(T);
        }
    }

    class TriggerEntryCollection : ITriggerEntry
    {
        public TriggerEntryCollection(IPropertyEntry propertyEntry, TriggerPath path, PropertyChangedEventHandler handler)
        {
            //var propertyEntry = classParser.GetPropertyEntry(path.PropertyName);
            propertyEntry.PropertyChanged += handler;

            _onDispose = () => propertyEntry.PropertyChanged -= handler;

            if (path != null)
            {
                propertyEntry.Link(NextHandler);
                _onDispose = () => propertyEntry.Unlink(NextHandler);

                void NextHandler(object sender, RegisterValueEventArgs arg)
                {
                    if (arg.OldValue != null && _next.TryRemove(arg.OldValue, out var oldTrigger))
                    {
                        oldTrigger.Dispose();
                    }

                    if (arg.NewValue != null)
                    {
                        _next.GetOrAdd(arg.NewValue,
                            o => NotifyFactory.GetParser(o)?.GetTrigger(path, handler));
                    }
                }
            }
        }
        private readonly ConcurrentDictionary<object,ITriggerEntry> _next = new ConcurrentDictionary<object, ITriggerEntry>();
        private readonly Action _onDispose;

        public void Dispose()
        {
            foreach (var triggerEntry in _next.Values)
            {
                triggerEntry.Dispose();
            }
            _onDispose?.Invoke();
        }
    }
}
