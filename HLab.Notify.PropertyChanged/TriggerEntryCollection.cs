using HLab.Notify.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using HLab.Notify.PropertyChanged.NotifyParsers;

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
        public TriggerEntryCollection(IPropertyEntry propertyEntry, TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            //var propertyEntry = classParser.GetPropertyEntry(path.PropertyName);
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
                        if (a.OldValue != null && _next.TryRemove(a.OldValue, out var oldTrigger))
                        {
                            oldTrigger.Dispose();
                        }

                        if (a.NewValue != null)
                        {
                            _next.GetOrAdd(a.NewValue,
                                o => NotifyClassHelper.GetHelper(o)?.GetTrigger(path, handler));
                        }
                    }
                }
            }
        }
        private readonly ConcurrentDictionary<object,ITriggerEntry> _next = new ConcurrentDictionary<object, ITriggerEntry>();
        private readonly Action _onDispose;

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            // Dispose of managed resources here.
            if (disposing)
            {
                foreach (var triggerEntry in _next.Values)
                {
                    triggerEntry.Dispose();
                }
                _onDispose?.Invoke();
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.

            _disposed = true;
        }
    }
}
