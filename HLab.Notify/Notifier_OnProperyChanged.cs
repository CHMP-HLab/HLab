using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using HLab.Base;
using HLab.Notify.Annotations;

namespace HLab.Notify
{
    public partial class Notifier<TClass>
    {
        private WeakReference<TClass> _target;

        public TClass Target
        {
            get
            {
                if (_target.TryGetTarget(out var target))
                    return target;

                throw new Exception("No target available");
            }
        }
        INotifyPropertyChanged INotifier.Target => Target;

        public void Add(PropertyChangedEventHandler handler) 
            => PropertyChanged += handler;
        public void Remove(PropertyChangedEventHandler handler) 
            => PropertyChanged -= handler;

        private Suspender _suspender;
        public Suspender Suspend => _suspender ?? (_suspender = new Suspender(OnPropertyChanged));


        private event PropertyChangedEventHandler PropertyChanged;

        private readonly ConcurrentDictionary<INotifierPropertyEntry, NotifierPropertyChangedEventArgs> _queue = new ConcurrentDictionary<INotifierPropertyEntry, NotifierPropertyChangedEventArgs>();

        //Enqueue or update property change event
        public void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            if(eventArgs is NotifierPropertyChangedEventArgs notifierEventArgs)
            using (Suspend.Get())
            {
                var arg = _queue.GetOrAdd(notifierEventArgs.Entry, e => notifierEventArgs);

                if (!ReferenceEquals(arg,notifierEventArgs)) arg.NewValue = notifierEventArgs.NewValue;
            }
        }


        private void OnPropertyChanged()
        {
            // BUG: crashed once on _queue.Keys.First() because queue was empty maybe I should lock
            if (PropertyChanged == null)
            {
                _queue.Clear();
                return;
            }

            while (_queue.Count>0)
            if(_queue.TryRemove(_queue.Keys.First(),out var args))
            {
                _eventHandlerService.Invoke(PropertyChanged, Target, args);
            }
        }
    }
}
