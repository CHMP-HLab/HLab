using System;
using System.Collections;
using HLab.Notify.Annotations;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace HLab.Notify.PropertyChanged
{
    public class NotifyClassParser : INotifyClassParser
    {
        private readonly object _target;
        public NotifyClassParser(object target)
        {
            _target = target;
            if(_target is INotifyPropertyChanged tpc)
                tpc.PropertyChanged += TargetOnPropertyChanged;

            if (_target is INotifyCollectionChanged tcc)
            {
                _dict.GetOrAdd("Item", n => new CollectionEntry(tcc));
            }
        }

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
                RegisterValue?.Invoke(sender, new RegisterValueEventArgs(null, item));
                PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Item"));
            }

            private void RemoveItem(object sender, object item)
            {
                RegisterValue?.Invoke(sender, new RegisterValueEventArgs(item, null));
                PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Item"));
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
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public void InitialRegisterValue()
            {

            }

            public event PropertyChangedEventHandler PropertyChanged;
            public event RegisterValueEventHandler RegisterValue;
            public void Link(RegisterValueEventHandler handler)
            {
                if (_target is IEnumerable e)
                    foreach (var obj in e) handler(this, new RegisterValueEventArgs(null, obj));

                RegisterValue += handler;
            }

            public void Unlink(RegisterValueEventHandler handler)
            {
                if (_target is IEnumerable e)
                    foreach (var obj in e) handler(this, new RegisterValueEventArgs(obj, null));

                RegisterValue -= handler;
            }

            public void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                PropertyChanged?.Invoke(sender,args);
            }

            public ITriggerEntry GetTrigger(TriggerPath path, PropertyChangedEventHandler handler) 
                => new TriggerEntryCollection(this,path,handler);

            public bool Linked => RegisterValue != null;
        }

        class PropertyEntry : IPropertyEntry
        {
            private readonly PropertyInfo _property;
            private object _value;
            private readonly object _target;
            public string Name { get; }

            public bool Linked => RegisterValue != null;

            public PropertyEntry(object target, string name)
            {
                _target = target;
                Name = name;
                PropertyInfo property = null;
                var type = target.GetType();
                while (property == null && type!=null)
                {
                    property = type.GetProperty(name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                    type = type.BaseType;
                }

                _property = property ?? throw new Exception("Property not found : " + name + " in " + target.GetType());
                //SetValue(target);
            }

            private bool SetValue()
            {
                try
                {
                    _value = _property?.GetValue(_target);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            public event RegisterValueEventHandler RegisterValue;

            public void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                var oldValue = _value;
                if (SetValue())
                {
                    if (Equals(oldValue,_value)) return;

                    PropertyChanged?.Invoke(sender, args);
                    RegisterValue?.Invoke(sender,new RegisterValueEventArgs(oldValue,_value));
                }
            }
            public void InitialRegisterValue()
            {
                var oldValue = _value;
                if (SetValue())
                {
                    if (Equals(oldValue, _value)) return;

                    //PropertyChanged?.Invoke(sender, args);
                    RegisterValue?.Invoke(_target, new RegisterValueEventArgs(oldValue, _value));
                }
            }
            public void Link(RegisterValueEventHandler handler)
            {
                if (_value != null)
                    handler(this, new RegisterValueEventArgs(null, _value));

                RegisterValue += handler;
            }

            public void Unlink(RegisterValueEventHandler handler)
            {
                if (_value != null)
                    handler(this, new RegisterValueEventArgs(_value, null));

                RegisterValue -= handler;
            }
            public ITriggerEntry GetTrigger(TriggerPath path, PropertyChangedEventHandler handler)
            {
                return new TriggerEntryNotifier(this, path, handler);
            }

        }


        private readonly ConcurrentDictionary<string,IPropertyEntry> _dict = new ConcurrentDictionary<string, IPropertyEntry>();

        public IPropertyEntry GetPropertyEntry(string name)
        {
            return _dict.GetOrAdd(name, n => new PropertyEntry(_target, n));
        }

        public ITriggerEntry GetTrigger(TriggerPath path, PropertyChangedEventHandler handler)
        {
            return GetPropertyEntry(path.PropertyName).GetTrigger(path.Next, handler);
        }

        public IEnumerable<IPropertyEntry> LinkedProperties()
        {
            foreach (var p in _dict.Values)
            {
                if(p.Linked) yield return p;
            }
        }

        private void TargetOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_dict.TryGetValue(args.PropertyName, out var propertyEntry))
            {
                propertyEntry.OnPropertyChanged(_target,args);
            }
        }
    }
}