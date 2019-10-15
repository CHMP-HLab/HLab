using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HLab.Notify.PropertyChanged
{
    public class Persister
    {
        protected readonly ConcurrentBag<PropertyInfo> Dirty = new ConcurrentBag<PropertyInfo>();
        public bool IsDirty => Dirty.Count > 0;

        public bool Loading { get; private set; } = false;

        protected readonly object Target;
        public Persister(INotifyPropertyChanged target)
        {
            Target = target;
            foreach (var property in Target.GetType().GetProperties())
            {
                foreach (var unused in property.GetCustomAttributes().OfType<PersistentAttribute>())
                {
                    Dirty.Add(property);
                }
            }
            target.PropertyChanged += Obj_PropertyChanged;
        }

        private void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var property = Target.GetType().GetProperty(e.PropertyName);
            if (property == null) return;

            foreach (var attr in property.GetCustomAttributes().OfType<PersistentAttribute>())
            {
                switch (attr.Persistency)
                {
                    case Persistency.OnChange:
                        Save(property);
                        break;
                    case Persistency.OnSave:
                        Dirty.Add(property);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual void Save()
        {
            while (!Dirty.IsEmpty)
            {
                if (Dirty.TryTake(out var e))
                {
                    Save(e);
                }
            }
        }

        public virtual void Load()
        {
            Loading = true;

            foreach (var property in Target.GetType().GetProperties())
            {
                foreach (var unused in property.GetCustomAttributes().OfType<PersistentAttribute>())
                {
                    Load(property);
                }
            }
            while(Dirty.TryTake(out var unused2));

            Loading = false;
        }

        protected void Load(PropertyInfo property)
        {
            if(Load(property.Name, out object value))
            {
                if (value.GetType() == property.PropertyType)
                {
                    property.SetValue(Target, value);
                    return;
                }

                if (property.PropertyType == typeof(bool))
                {
                    switch(value.ToString().ToLower())
                    {
                        case "1":
                        case "true":
                        case "on":
                            property.SetValue(Target, true);
                            break;
                        default:
                            property.SetValue(Target, false);
                            break;
                    }
                    return;
                }

                if(property.PropertyType == typeof(DateTime))
                {
                    var date = DateTime.Parse(value.ToString());
                    property.SetValue(Target, date);
                    return;
                }

                if(property.PropertyType.IsEnum && value is string stringValue)
                {
                    var v = Enum.Parse(property.PropertyType, stringValue);
                    property.SetValue(Target, v);
                }
            }
        }

        protected virtual bool Load(string propertyName, out object value)
        {
            value = new object();
            return false;
        }

        protected void Save(PropertyInfo property)
        {
            Save(property.Name, property.GetValue(Target));
        }

        protected void Save(string entry)
        {
            Save(GetType().GetProperty(entry));
        }

        protected virtual void Save(string entry, object value)
        { }

    }
}