using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public class NotifyConfigurator { }

    public delegate NotifyConfigurator<TClass, T> NotifyConfiguratorFactory<TClass, T>(NotifyConfigurator<TClass, T> c)
        where TClass : class;




    public class NotifyConfigurator<TClass, T> : NotifyConfigurator
        where TClass : class
    {
        internal readonly List<TriggerEntry> Triggers = new List<TriggerEntry>();
        internal TriggerEntry CurrentTrigger = new TriggerEntry();

        internal class TriggerEntry
        {
            public List<Func<TClass,bool>> WhenList { get; } = new List<Func<TClass, bool>>();
            public List<TriggerPath> TriggerOnList { get; } = new List<TriggerPath>();
            public Action<TClass, T> Action { get; set; }
        }

        public class Activator
        {
            public Action<TClass, INotifyClassHelper, T> Action { get; internal set; } = null;
            public  List<string> DependsOn { get; } = new List<string>();
            public  string PropertyName { get; internal set; }
            public Action<TClass, T> UpdateAction { get; internal set; }
        }

        private Activator _activator = new Activator();

        public NotifyConfigurator(){}


        public NotifyConfigurator<TClass, T> Name(string name)
        {
            _activator.PropertyName = name;
            return this;
        }



        public NotifyConfigurator<TClass, T> AddTriggerExpression(Expression expr)
        {
            CurrentTrigger.TriggerOnList.Add(new TriggerPath(expr));
            return this;
        }
        public NotifyConfigurator<TClass, T> OnEvent(Action<TClass, EventHandler> action)
        {
            // TODO : _currentTrigger.TriggerOnList.Add(new TriggerPath(expr));
            return this;
        }
        public NotifyConfigurator<TClass, T> On(params string[] path)
        {
            CurrentTrigger.TriggerOnList.Add(new TriggerPath(path));
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

        public NotifyConfigurator<TClass, T> Update()
        {
            return Do(_activator.UpdateAction);
        }

        public NotifyConfigurator<TClass, T> Do(Action<TClass> action)
        {   
            Func<TClass,bool> when = null;
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

            if(when==null)
                CurrentTrigger.Action = (parent,child) => action(parent);
            else
                CurrentTrigger.Action = (parent, child) =>
                {
                    if(when(parent))
                        action(parent);
                };
                

            Triggers.Add(CurrentTrigger);
            CurrentTrigger = new TriggerEntry();
            return this;
        }

        public Action<TClass, T> GetDoWhenAction(Action<TClass, T> action)
        {
            Func<TClass,bool> when = null;
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

            if(when==null)
                return action;
            else
                return (parent, child) =>
                {
                    if(when(parent))
                        action(parent,child);
                };

        }



        public NotifyConfigurator<TClass, T> Do(Action<TClass, T> action)
        {
            CurrentTrigger.Action = GetDoWhenAction(action);

            Triggers.Add(CurrentTrigger);
            CurrentTrigger = new TriggerEntry();
            return this;
        }



        public Activator Compile(string name)
        {
            if (_activator.PropertyName == null) _activator.PropertyName = NotifyHelper.GetNameFromCallerName(name);

            foreach (var trigger in Triggers)
            {
                // If the trigger does not contain triggerOn entries the action will occur at initialization 
                if(trigger.TriggerOnList.Count==0) trigger.TriggerOnList.Add(null);

                foreach (var path in trigger.TriggerOnList)
                {
                    if(path != null && !string.IsNullOrWhiteSpace(path.PropertyName) && ! _activator.DependsOn.Contains(path.PropertyName)) _activator.DependsOn.Add(path.PropertyName);

                    var action = trigger.Action;

                    if(path==null || string.IsNullOrWhiteSpace(path.PropertyName))
                        _activator.Action += (parent, parser, property) => action(parent, property);
                    else
                        _activator.Action += (parent, parser, property) =>
                        {
                            parser.GetTrigger(path, (s, a) => { action(parent, property); });
                        };
                }
            }

            var activator = _activator;
            _activator = null;
            return activator;
        }
    }
}
