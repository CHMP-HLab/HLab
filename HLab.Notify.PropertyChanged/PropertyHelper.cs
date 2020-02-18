using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using HLab.Base;

namespace HLab.Notify.PropertyChanged
{
    public class NotifyHelper
    {
        public static IEventHandlerService EventHandlerService { get; set; } = new EventHandlerService();
        protected static string Name(string name)
        {
            if (!name.StartsWith("_") || name.Length <= 1) return name;

            var n = new StringBuilder();
            n.Append(name[1].ToString().ToUpper());
            if (name.Length > 2) n.Append(name.Substring(2));
            return n.ToString();
        }

        protected static readonly MethodInfo SetParent = typeof(IChildObject).GetMethod("SetParent", new Type[] { typeof(object),typeof(Action<PropertyChangedEventArgs>) });
    }
}