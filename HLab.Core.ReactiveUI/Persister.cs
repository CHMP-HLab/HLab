using System.ComponentModel;
using System.Reflection;

using ReactiveUI;

using HLab.Base;
using HLab.Notify.PropertyChanged;

namespace HLab.Core.ReactiveUI;

public class Persister : ReactiveObject
{
    protected ConcurrentHashSet<PropertyInfo> Dirty { get; } = new();
    public bool IsDirty
    {
        get => _isDirty;
        protected set => this.RaiseAndSetIfChanged(ref _isDirty, value);
    }
    bool _isDirty;

    public void Reset()
    {
        while (Dirty.TryTake(out var e))
        {
        }
        IsDirty = false;
    }

    public bool Loading { get; private set; } = false;

    protected object Target { get; }
    public Persister(object target, bool isDirty = true)
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
                    IsDirty = true;
                }
            }
        }
        if (target is INotifyPropertyChanged npc)
            npc.PropertyChanged += Obj_PropertyChanged;
    }

    protected virtual Persistency CheckPersistency(PropertyInfo property)
    {
        return property
            .GetCustomAttributes()
            .OfType<PersistentAttribute>()
            .Select(attr => attr.Persistency)
            .FirstOrDefault();
    }

    void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == null) throw new ArgumentNullException(nameof(e.PropertyName));

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
                IsDirty = true;
                break;
            case Persistency.None:
            default:
                break;
        }
    }

    public virtual bool Save()
    {
        while (Dirty.TryTake(out var e))
        {
            Save(e);
        }
        IsDirty = false;
        return true;
    }
    public virtual async Task<bool> SaveAsync()
    {
        while (Dirty.TryTake(out var e))
        {
            await SaveAsync(e);
        }
        IsDirty = false;
        return true;
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
        while (Dirty.TryTake(out var unused2)) ;
        IsDirty = false;

        Loading = false;
    }

    protected void Load(PropertyInfo property)
    {
        if (Load(property.Name, out object value))
        {
            if (value.GetType() == property.PropertyType)
            {
                property.SetValue(Target, value);
                return;
            }

            if (property.PropertyType == typeof(bool))
            {
                switch (value.ToString().ToLower())
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

            if (property.PropertyType == typeof(DateTime))
            {
                var date = DateTime.Parse(value.ToString());
                property.SetValue(Target, date);
                return;
            }

            if (property.PropertyType.IsEnum && value is string stringValue)
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
    protected async Task SaveAsync(PropertyInfo property)
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