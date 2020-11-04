using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
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
                if (ExtendedPropertyChanged == null) return;

                var oldValue = _value;
                _value = _property.GetValue(_target);
                if (Equals(oldValue,_value)) return;

                ExtendedPropertyChanged?.Invoke(sender,new ExtendedPropertyChangedEventArgs(args, oldValue,_value));
            }


            public void InitialRegisterValue(Type type)
            {
                if (_property.DeclaringType != type) return;
                if (ExtendedPropertyChanged == null) return;

                if (_target.GetType().GetProperty(Name).GetMethod != _property.GetMethod) return;

                var oldValue = _value;

                _value = _property.GetValue(_target);
                if (Equals(oldValue, _value)) return;

                ExtendedPropertyChanged.Invoke(_target, new ExtendedPropertyChangedEventArgs(Name, oldValue, _value));
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
            public ITriggerEntry GetTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
            {
                return new TriggerEntryNotifier(this, path, handler);
            }

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
}
