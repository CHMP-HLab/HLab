using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HLab.Base;

namespace HLab.Notify.PropertyChanged
{
    public class Persister :N<Persister>
    {
        protected ConcurrentHashSet<PropertyInfo> Dirty { get; } = new ConcurrentHashSet<PropertyInfo>();
        public bool IsDirty
        {
            get => _isDirty.Get();
            protected set => _isDirty.Set(value);
        }

        public void Reset()
        {
            while (Dirty.TryTake(out var e))
            {
            }
            IsDirty = false;
        }

        private readonly IProperty<bool> _isDirty = H.Property<bool>();

        public bool Loading { get; private set; } = false;

        protected object Target {get; }
        public Persister(object target,bool isDirty = true)
        {
            Target = target;

            if (isDirty)
            {
                foreach (var property in Target.GetType().GetProperties())
                {
                    var persistency = CheckPersistency(property);
                    if (persistency != Persistency.None)
                    {
                        Dirty.Add(property);
                        _isDirty.Set(true);
                    }
                }
            }
            if(target is INotifyPropertyChanged npc)
                npc.PropertyChanged += Obj_PropertyChanged;
        }

        protected virtual Persistency CheckPersistency(PropertyInfo property)
        {
            foreach (var attr in property.GetCustomAttributes().OfType<PersistentAttribute>())
            {
                return attr.Persistency;
            }

            return Persistency.None;
        }

        private void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Contains("Country"))
            { }

            var property = Target.GetType().GetProperty(e.PropertyName);
            if (property == null) return;

            var persistency = CheckPersistency(property);
            switch (persistency)
            {
                case Persistency.OnChange:
                    Save(property);
                    break;
                case Persistency.OnSave:
                    Dirty.Add(property);
                    _isDirty.Set(true);
                    break;
                default:
                    break;
            }
        }

        public virtual void Save()
        {
            while (Dirty.TryTake(out var e))
            {
                Save(e);
            }
            _isDirty.Set(false);
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
            _isDirty.Set(false);

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