#define DONT_USE_WEAK_HANDLERS


using HLab.Notify.Annotations;
using System;
using System.Collections.Generic;

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
        protected IPropertyEntry PropertyEntry;
        protected string Debug;

        protected EventHandler<ExtendedPropertyChangedEventArgs> Handler;
        public TriggerEntryCollection(IPropertyEntry propertyEntry, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            PropertyEntry = propertyEntry;
            Handler = handler;
            propertyEntry.ExtendedPropertyChanged += OnPropertyChanged;
        }

        public virtual void Dispose()
        {
            PropertyEntry.ExtendedPropertyChanged -= OnPropertyChanged;
        }
        private void OnPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
        {
            Handler?.Invoke(sender,e);
        }
    }
}
