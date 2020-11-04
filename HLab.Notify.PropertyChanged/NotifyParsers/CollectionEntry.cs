using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged.NotifyParsers
{
    class CollectionEntry : IPropertyEntry
    {
        private readonly INotifyCollectionChanged _target;
        public string Name => "Item";

        public CollectionEntry(INotifyCollectionChanged target)
        {
            _target = target;
            target.CollectionChanged += TargetOnCollectionChanged;
        }

        private void AddItem(object sender, object item)
        {
            ExtendedPropertyChanged?.Invoke(sender, new ExtendedPropertyChangedEventArgs("Item", null, item));
        }

        private void RemoveItem(object sender, object item)
        {
            ExtendedPropertyChanged?.Invoke(sender, new ExtendedPropertyChangedEventArgs("Item", item, null));
        }

        private void TargetOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewItems != null)
                        foreach (var item in args.NewItems)
                        {
                            AddItem(sender, item);
                        }
                    else throw new Exception("NewItems was null");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (args.OldItems != null)
                        foreach (var item in args.OldItems)
                        {
                            RemoveItem(sender, item);
                        }
                    else throw new Exception("OldItems was null");
                    break;

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    ExtendedPropertyChanged?.Invoke(sender, new ExtendedPropertyChangedEventArgs("Item", null, null));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitialRegisterValue(Type type)
        {

        }

        public event EventHandler<ExtendedPropertyChangedEventArgs> ExtendedPropertyChanged;
        //public event EventHandler<ExtendedPropertyChangedEventArgs> RegisterValue;
        public void Link(EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            if (_target is IEnumerable e)
                foreach (var obj in e) handler(this, new ExtendedPropertyChangedEventArgs("Item", null, obj));

            ExtendedPropertyChanged += handler;
        }

        public void Unlink(EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            if (_target is IEnumerable e)
                foreach (var obj in e) handler(this, new ExtendedPropertyChangedEventArgs("Item", obj, null));

            ExtendedPropertyChanged -= handler;
        }

        public void TargetPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            ExtendedPropertyChanged?.Invoke(sender, (args as ExtendedPropertyChangedEventArgs) ?? new ExtendedPropertyChangedEventArgs(args, null, null));
        }

        public ITriggerEntry GetTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
            => new TriggerEntryCollection(this, path, handler);

        public bool Linked => ExtendedPropertyChanged != null;

        public void Dispose()
        {
            if (ExtendedPropertyChanged == null) return;
            foreach (var d in ExtendedPropertyChanged.GetInvocationList())
            {
                if (d is EventHandler<ExtendedPropertyChangedEventArgs> h)
                    Unlink(h);
            }
        }
    }
}
