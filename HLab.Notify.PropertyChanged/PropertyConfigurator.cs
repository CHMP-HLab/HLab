using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyConfigurator_old
    {
        public PropertyChangedEventArgs EventArgs { get; protected set; }

    }
    public class PropertyConfigurator_old<TClass> : PropertyConfigurator_old
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TClass">Class holding this property</typeparam>
    /// <typeparam name="T">Type returned by the property</typeparam>
    public class PropertyConfigurator_old<TClass, T> : PropertyConfigurator_old<TClass>
    where TClass : class
    {
        public PropertyConfigurator_old(string name)
        {
            EventArgs = new PropertyChangedEventArgs(name);
        }

        internal readonly List<TriggerEntry> Triggers = new List<TriggerEntry>();
        internal class TriggerEntry
        {
            public List<Func<TClass,bool>> WhenList { get; } = new List<Func<TClass, bool>>();
            public List<TriggerPath> TriggerOnList { get; } = new List<TriggerPath>();
            public Action<TClass, PropertyHolder<T>> Action { get; set; }
        }
        private TriggerEntry _currentTrigger = new TriggerEntry();
        public Func<object, T> DefaultSetter { get; private set; } = c => default(T);

        public PropertyConfigurator_old<TClass, T> TriggerExpression(Expression expr)
        {
            _currentTrigger.TriggerOnList.Add(new TriggerPath(expr));
            return this;
        }
        public PropertyConfigurator_old<TClass, T> OnEvent(Action<TClass, EventHandler> action)
        {
            // TODO : _currentTrigger.TriggerOnList.Add(new TriggerPath(expr));
            return this;
        }
        public PropertyConfigurator_old<TClass, T> On(params string[] path)
        {
            _currentTrigger.TriggerOnList.Add(new TriggerPath(path));
            return this;
        }
        public PropertyConfigurator_old<TClass, T> Set(Func<TClass, T> setter)
        {
            if (_currentTrigger.WhenList.Count > 0)
            {
                var trigger = _currentTrigger;

                _currentTrigger.Action = (target, property) =>
                {
                    if(trigger.WhenList.All(e => e(target)))
                        property.PropertyValue.Set(setter(target));
                };

            }
            else 
                _currentTrigger.Action = (target, property) => { property.PropertyValue.Set(setter(target)); };

            Triggers.Add(_currentTrigger);
            _currentTrigger = new TriggerEntry();
            DefaultSetter = c => setter((TClass)c);
            return this;
        }
        public PropertyConfigurator_old<TClass, T> When(Func<TClass, bool> when)
        {
            _currentTrigger.WhenList.Add(when);
            return this;
        }
        public PropertyConfigurator_old<TClass, T> Do(Action<TClass, PropertyHolder<T>> action)
        {
            _currentTrigger.Action = action;
            Triggers.Add(_currentTrigger);
            _currentTrigger = new TriggerEntry();
            return this;
        }


        public Action<TClass, PropertyHolder<T>> Register { get; private set; }

        public PropertyConfigurator_old<TClass, T> Compile()
        {
            Action<INotifyClassParser, TClass, PropertyHolder<T>> register = null;

            foreach (var trigger in Triggers)
            foreach (var path in trigger.TriggerOnList)
            {
                var action = trigger.Action;
                register += (parser, parent, property) =>
                {
                    //NotifyFactory.GetParser(parent)
                    parser.GetTrigger(path, (s, a) => { action(parent, property); });
                };
            }
            if(register!=null)
                Register = (parent, property) => register(NotifyFactory.GetParser(parent), parent, property);
            else
                Register = null;

//            
            return this;
        }
    }
}