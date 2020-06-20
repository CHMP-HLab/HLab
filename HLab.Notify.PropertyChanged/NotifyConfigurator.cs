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
        public Action<TClass, T> UpdateAction { get; private set; }

        public NotifyConfigurator<TClass, T> TriggerExpression(Expression expr)
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
            UpdateAction = action;
            return this;
        }

        public NotifyConfigurator<TClass, T> Update() => Do(UpdateAction);

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
        public NotifyConfigurator<TClass, T> Do(Action<TClass, T> action)
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
                CurrentTrigger.Action = action;
            else
                CurrentTrigger.Action = (parent, child) =>
                {
                    if(when(parent))
                        action(parent,child);
                };
                

            Triggers.Add(CurrentTrigger);
            CurrentTrigger = new TriggerEntry();
            return this;
        }


        public Action<TClass, INotifyClassParser, T> Compile()
        {
            Action<TClass, INotifyClassParser, T> activator = null;

            foreach (var trigger in Triggers)
            {
                // If the trigger does not contain triggerOn entries the action will occur at initialization 
                if(trigger.TriggerOnList.Count==0) trigger.TriggerOnList.Add(null);

                foreach (var path in trigger.TriggerOnList)
                {
                    var action = trigger.Action;

                    if(string.IsNullOrWhiteSpace(path?.PropertyName))
                        activator += (parent, parser, property) => action(parent, property);
                    else
                        activator += (parent, parser, property) =>
                        {
                            //NotifyFactory.GetParser(parent)
                            parser.GetTrigger(path, (s, a) => { action(parent, property); });
                        };
                }
            }

            return activator;

        }


    }
}
