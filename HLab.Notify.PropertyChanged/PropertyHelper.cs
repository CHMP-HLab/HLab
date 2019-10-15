using System;
using System.ComponentModel;
using System.Reflection;
using HLab.Base;

namespace HLab.Notify.PropertyChanged
{
    public class NotifyHelper
    {
        public static IEventHandlerService EventHandlerService { get; set; } = new EventHandlerService();
        protected static string Name(string name) => (name.StartsWith("_"))?(name[1].ToString()).ToUpper() + name.Substring(2):name;
        protected static readonly MethodInfo SetParent = typeof(IChildObject).GetMethod("SetParent", new Type[] { typeof(object),typeof(Action<PropertyChangedEventArgs>) });
    }
}