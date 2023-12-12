using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace HLab.Notify.PropertyChanged.NotifyHelpers;

public partial class NotifyClassHelper
{
    internal class PropertyEntry : IPropertyEntry
    {
        public event EventHandler<ExtendedPropertyChangedEventArgs> ExtendedPropertyChanged;
        public bool IsLinked() => ExtendedPropertyChanged != null;
        public string Name { get; }

        readonly PropertyInfo _property;
        object _value;
        readonly object _target;

        public PropertyEntry(object target, string name)
        {
            _target = target;
            Name = name;

            _property = GetProperty(target.GetType(),name);
        }

        static PropertyInfo GetProperty(Type type, string name)
        {
            var property = type.GetProperty(name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            var subType = type;

            while (property == null)
            {
                subType = subType.BaseType;
                if (subType != null)
                {
                    property = subType.GetProperty(name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                }
                else
                {
                    property = GetInterfaceProperty(type, name);

                    if (property == null)
                        throw new Exception("Property not found : " + name + " in " + type);
                }
            }

            return property;
        }

        static PropertyInfo GetInterfaceProperty(Type type, string name)
        {
            var interfaces = type.GetInterfaces();

            foreach (var iface in interfaces)
            {
                var prop = iface.GetProperty(name);
                if (prop != null) return prop;
            }
            return null;
        }


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

        List<EventHandler<ExtendedPropertyChangedEventArgs>> _triggerEntries = new();

        public ITriggerEntry BuildTrigger(EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            _triggerEntries.Add(handler);
            var entry = new TriggerEntryNotifier(this, handler);
            return entry;
        }

        public ITriggerEntry BuildTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            var entry = new TriggerEntryNotifierWithPath(this, path, handler);
            //_triggerEntries.Add(entry);
            return entry;
        }

        public override string ToString() => _target.ToString() + " : " + Name;

    }

}