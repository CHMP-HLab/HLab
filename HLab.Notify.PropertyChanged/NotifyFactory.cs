using System.Collections.Generic;
using HLab.Notify.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged
{
    public class RegisterValueEventArgs
    {
        public RegisterValueEventArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object OldValue { get; }
        public object NewValue { get; }
    }

    public delegate void RegisterValueEventHandler(object sender, RegisterValueEventArgs e);

    public interface INotifyClassParser
    {
        IPropertyEntry GetPropertyEntry(string name);
        ITriggerEntry GetTrigger(TriggerPath path, PropertyChangedEventHandler handler);
        IEnumerable<IPropertyEntry> LinkedProperties();
    }

    public static class NotifyFactory
    {
        private static readonly ConditionalWeakTable<object, INotifyClassParser> 
            Cache = new ConditionalWeakTable<object, INotifyClassParser>();
        public static INotifyClassParser GetExistingParser(object target)
        {
            return Cache.TryGetValue(target, out var p) ? p : null;
        }

        public static INotifyClassParser GetParser(object target)
        {
                return Cache.GetValue(target, (o) => new NotifyClassParser(o));
        }
    }
}
