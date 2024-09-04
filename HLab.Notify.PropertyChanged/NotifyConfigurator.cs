using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HLab.Notify.PropertyChanged;

public class NotifyConfigurator { }

public delegate NotifyConfigurator<TClass, T> NotifyConfiguratorFactory<TClass, T>(NotifyConfigurator<TClass, T> c)
    where TClass : class, INotifyPropertyChangedWithHelper
    where T : class, IChildObject
;


public abstract class PropertyActivator
{
    public string PropertyName { get; internal set; }
    public List<string> DependsOn { get; } = new();
    public abstract void Activate(INotifyPropertyChangedWithHelper parent, IChildObject child);
    public abstract void Update(IChildObject child);
}
public abstract class PropertyActivator<T> : PropertyActivator
{
    public abstract void Activate(INotifyPropertyChangedWithHelper parent, T child);
    public abstract void Update(T child);
}

public class NotifyConfigurator<TClass, T> : NotifyConfigurator
    where TClass : class, INotifyPropertyChangedWithHelper
    where T : class, IChildObject
{
    readonly List<TriggerEntry> Triggers = new();
    internal TriggerEntry CurrentTrigger = new();

    internal class TriggerEntry
    {
        public List<Func<TClass, bool>> WhenList { get; } = new();
        public List<TriggerPath> TriggerOnList { get; } = new();
        public Action<TClass, T> Action { get; set; }
    }

    public class Activator : PropertyActivator
    {
        static readonly Action<TClass, T> DefaultAction = (parent, child) => { };
        internal Action<TClass, T> Action { get; set; } = DefaultAction;
        internal Action<TClass, T> UpdateAction { get; set; } = DefaultAction;

        internal List<object> Triggers = new();

        public override void Activate(INotifyPropertyChangedWithHelper parent, IChildObject child)
        {
            Action((TClass)parent, (T)child);
        }

        public override void Update(IChildObject child)
        {
            UpdateAction((TClass)child.Parent, (T)child);
        }
    }

    Activator _activator = new();

    public NotifyConfigurator<TClass, T> Name(string name)
    {
        _activator.PropertyName = name;
        return this;
    }

    public override string ToString()
    {
        return typeof(T).Name + " : " + typeof(TClass).Name + "." + _activator.PropertyName;
    }

    public NotifyConfigurator<TClass, T> AddTriggerExpression(Expression expr)
    {
        CurrentTrigger.TriggerOnList.Add(TriggerPath.Factory(expr));
        return this;
    }
    public NotifyConfigurator<TClass, T> OnEvent(Action<TClass, EventHandler> action)
    {
        // TODO : _currentTrigger.TriggerOnList.Add(new TriggerPath(expr));
        return this;
    }
    public NotifyConfigurator<TClass, T> On(params string[] path)
    {
        CurrentTrigger.TriggerOnList.Add(TriggerPath.Factory(path));
        return this;
    }
    public NotifyConfigurator<TClass, T> When(Func<TClass, bool> when)
    {
        CurrentTrigger.WhenList.Add(when);
        return this;
    }

    public NotifyConfigurator<TClass, T> Init(Action<TClass, T> action)
    {
        _activator.UpdateAction = action;
        return this;
    }


    public NotifyConfigurator<TClass, T> Do(Action<TClass, T> action)
    {
        CurrentTrigger.Action = GetDoWhenAction(action);

        Triggers.Add(CurrentTrigger);
        CurrentTrigger = new TriggerEntry();
        return this;
    }

    public NotifyConfigurator<TClass, T> Do(Action<TClass> action) => Do((p, c) => action(p));
    public NotifyConfigurator<TClass, T> Update() => Do(_activator.UpdateAction);

    public Action<TClass, T> GetDoWhenAction(Action<TClass, T> action)
    {
        Func<TClass, bool> when = null;
        foreach (var w in CurrentTrigger.WhenList)
        {
            if (when == null)
                when = w;
            else
            {
                var old = when;
                when = c => old(c) && w(c);
            }
        }

        if (when == null)
            return action;

        return (parent, child) =>
        {
            if (when(parent))
                action(parent, child);
        };

    }






    public Activator Compile(string name)
    {
        _activator.PropertyName ??= NotifyHelper.GetNameFromCallerName(name);

        foreach (var trigger in Triggers)
        {
            // If the trigger does not contain triggerOn entries the action will occur at initialization 
            if (trigger.TriggerOnList.Count == 0) trigger.TriggerOnList.Add(null);

            foreach (var path in trigger.TriggerOnList)
            {
                if (path != null && !string.IsNullOrWhiteSpace(path.PropertyName) && !_activator.DependsOn.Contains(path.PropertyName)) _activator.DependsOn.Add(path.PropertyName);

                var action = trigger.Action;

                _activator.Triggers.Add(action);

                if (path == null || string.IsNullOrWhiteSpace(path.PropertyName))
                    _activator.Action += action;
                else
                {

                    _activator.Action += (parent, property) =>
                    {
                        ITriggerEntry trigger = null;
                        var wr = new WeakReference<T>(property);
                        trigger = path.GetTrigger(parent.ClassHelper, Handler);
                        property.OnDispose(() => trigger.Dispose());


                        void Handler(object s, ExtendedPropertyChangedEventArgs a)
                        {
                            if (wr.TryGetTarget(out var pp))
                                action((TClass)pp.Parent, pp);
                            else
                            {
                                trigger?.Dispose();
                            }
                        }
                    };

                }
            }
        }

        var activator = _activator;
        _activator = null;
        return activator;
    }
}