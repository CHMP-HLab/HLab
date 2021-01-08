using HLab.Notify.Annotations;
using System;
using System.Diagnostics;


namespace HLab.Notify.PropertyChanged
{

    public class TriggerEntryNotifier : ITriggerEntry
    {
        protected readonly IPropertyEntry PropertyEntry;
        protected readonly WeakReference<EventHandler<ExtendedPropertyChangedEventArgs>> Handler;

        #if DEBUG
        private readonly string _targetName;
        #endif

        public TriggerEntryNotifier(IPropertyEntry propertyEntry, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            #if DEBUG
                _targetName = handler.Target.ToString();
            #endif

            PropertyEntry = propertyEntry;
            if(handler!=null)
            {
                Handler = new(handler);
                propertyEntry.ExtendedPropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
        {
            if(Handler.TryGetTarget(out var handler))
            {
                handler.Invoke(sender,e);
            }
            else
            {}
        }

        public virtual void Dispose()
        {
            PropertyEntry.ExtendedPropertyChanged -= OnPropertyChanged;
        }

        public override string ToString()
        {
            return $"{PropertyEntry.Name}";
        }
    }
}
