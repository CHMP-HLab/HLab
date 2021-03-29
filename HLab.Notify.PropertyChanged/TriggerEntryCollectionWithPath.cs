#define DONT_USE_WEAK_HANDLERS

using System;
using System.Collections.Concurrent;
using HLab.Notify.Annotations;
using NotifyClassHelper = HLab.Notify.PropertyChanged.NotifyHelpers.NotifyClassHelper;

namespace HLab.Notify.PropertyChanged
{
    class TriggerEntryCollectionWithPath : TriggerEntryCollection
    {
        private readonly TriggerPath _path;

        public TriggerEntryCollectionWithPath(IPropertyEntry propertyEntry, TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
            : base(propertyEntry, handler)
        {
            _path = path;
            PropertyEntry.Link(OnPropertyChangedWithPath);
        }

        private void OnPropertyChangedWithPath(object sender, ExtendedPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && _next.TryRemove(e.OldValue, out var oldTrigger))
            {
                oldTrigger.Dispose();
            }
            if (e.NewValue != null)
            {
                var handler = Handler;

                _next.GetOrAdd(e.NewValue,
                    o => _path.GetTriggerB(NotifyClassHelper.GetHelper(o),handler));
            }
        }




        private readonly ConcurrentDictionary<object, ITriggerEntry> _next = new ConcurrentDictionary<object, ITriggerEntry>();
        public override void Dispose()
        {
            PropertyEntry.Unlink(OnPropertyChangedWithPath);
            foreach (var triggerEntry in _next.Values)
            {
                triggerEntry.Dispose();
            }
        }
    }
}