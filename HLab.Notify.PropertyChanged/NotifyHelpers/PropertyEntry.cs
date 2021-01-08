using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged.NotifyParsers
{
    public partial class NotifyClassHelper
    {
        internal class PropertyEntry : IPropertyEntry
        {
            private readonly PropertyInfo _property;
            private object _value;
            private readonly object _target;
            public string Name { get; }

            public bool Linked => ExtendedPropertyChanged != null;

            public PropertyEntry(object target, string name)
            {
                _target = target;
                Name = name;
                var type = target.GetType();

                var property = type.GetProperty(name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                while (property == null)
                {
                    type = type.BaseType;
                    if (type != null)
                    {
                        property = type.GetProperty(name,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    }
                    else throw new Exception("Property not found : " + name + " in " + target.GetType());
                }

                _property = property;
            }

            public event EventHandler<ExtendedPropertyChangedEventArgs> ExtendedPropertyChanged;

            public void TargetPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                var value = _property.GetValue(_target);
                var oldValue = Interlocked.Exchange(ref _value, value);

                if (ExtendedPropertyChanged == null) return;
                if (ReferenceEquals(oldValue, value)) return;

                ExtendedPropertyChanged(sender, new ExtendedPropertyChangedEventArgs(args, oldValue, value));
            }


            public void InitialRegisterValue(Type type)
            {
                if (_property.DeclaringType != type) return;
                //if (ExtendedPropertyChanged == null) return;

                if (_target.GetType().GetProperty(Name).GetMethod != _property.GetMethod) return;

                var value = _property.GetValue(_target);
                var oldValue = Interlocked.Exchange(ref _value, value);

                if (ReferenceEquals(oldValue, value)) return;

                ExtendedPropertyChanged?.Invoke(_target, new ExtendedPropertyChangedEventArgs(Name, oldValue, _value));
            }

            public void Link(EventHandler<ExtendedPropertyChangedEventArgs> handler)
            {
                _value = _property.GetValue(_target);
                if (_value != null)
                    handler(this, new ExtendedPropertyChangedEventArgs(Name, null, _value));

                ExtendedPropertyChanged += handler;
            }

            public void Unlink(EventHandler<ExtendedPropertyChangedEventArgs> handler)
            {
                if (_value != null)
                    handler(this, new ExtendedPropertyChangedEventArgs(Name, _value, null));

                ExtendedPropertyChanged -= handler;
            }
            
            private List<EventHandler<ExtendedPropertyChangedEventArgs>> _triggerEntries = new();

            public ITriggerEntry GetTrigger(EventHandler<ExtendedPropertyChangedEventArgs> handler)
            {
                _triggerEntries.Add(handler);
                var entry = new TriggerEntryNotifier(this, handler);
                return entry;
            }

            //public ITriggerEntry GetTrigger(Action<object, ExtendedPropertyChangedEventArgs> handler)
            //{
            //    var entry = new TriggerEntryNotifier(this, handler);
            //    _triggerEntries.Add(entry);
            //    return entry;
            //}
            public ITriggerEntry GetTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
            {
                var entry = new TriggerEntryNotifierWithPath(this, path, handler);
                //_triggerEntries.Add(entry);
                return entry;
            }

            public override string ToString() => _target.ToString() + " : " + Name ;

        }

    }
}
