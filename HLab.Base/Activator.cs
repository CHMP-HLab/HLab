using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace HLab.Base
{
    public abstract class ChildActivator
    {
        public abstract Type Class { get; }

        public abstract Action<TClass> GetActivator<TClass>();
        protected static ConcurrentBag<ChildActivator> Activators { get; } = new ConcurrentBag<ChildActivator>();

        public static void Add<TClass, TChild>(Action<TClass, TChild> action)
            where TClass : class
            where TChild : class
        {
            Activators.Add(new ChildActivator<TClass, TChild>(action));
        }

        public static IEnumerable<ChildActivator> GetActivators<TClass>()
        {
            return Activators.Where(t => t.Class.IsAssignableFrom(typeof(TClass)));
        }
    }


    public class ChildActivator<TClass,TChild> : ChildActivator
    where TClass : class
    where TChild : class
    {
        public ChildActivator(Action<TClass, TChild> action)
        {
            Action = action;
        }

        public override Type Class => typeof(TClass);
        private Action<TClass, TChild> Action { get; }

        public override Action<TClass1> GetActivator<TClass1>()
        {
            //return (parent) => Activator<TClass1>.GetActivator<TChild>((p,c)=>Action(p as TClass,c))(parent);
            return (parent) => Activator<TClass1>.GetCompiled<TChild>((p,c)=>Action(p as TClass,c))(parent);
        }
    }

    public class Activator<TClass>
    {
        public static void Activate(TClass c) => ActivateInternal(c);

        private static Action<TClass> ActivateInternal { get; } = GetActivator();

        protected static Action<TClass> GetActivator()
        {
            Action<TClass> action = c => { };
            var list = ChildActivator.GetActivators<TClass>();
            foreach (var act in list)
            {
                action += act.GetActivator<TClass>();
            }

            return action;
        }

        public static Action<TClass> GetActivator<TChild>(Action<TClass, TChild> action)
        {


            Action<TClass> act = c => {};
            var type = typeof(TClass);
            while (type != null)
            {
                var list = type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                           BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var f in list)
                {
                    switch (f)
                    {
                        case FieldInfo fieldInfo:
                            if (!typeof(TChild).IsAssignableFrom(fieldInfo.FieldType)) continue;
                            act += c => action(c, (TChild) fieldInfo.GetValue(c)); 
                            break;
                        case PropertyInfo propertyInfo:
                            if (!typeof(TChild).IsAssignableFrom(propertyInfo.PropertyType)) continue;
                            act += c => action(c, (TChild)propertyInfo.GetValue(c));
                            break;
                    }
                }
                type = type.BaseType;
            }

            return act;
        }
        public static Action<TClass> GetCompiled<TChild>(Action<TClass, TChild> expression)
        {
            DynamicMethod dm =
                new DynamicMethod($"{Guid.NewGuid()}", typeof(void), new[] { typeof(TClass) }, typeof(TClass), true);

            ILGenerator il = dm.GetILGenerator();

            //var action = expression.Compile();
            // LocalBuilder a = il.DeclareLocal(typeof(TChild));


            var type = typeof(TClass);
            while (type != null)
            {
                foreach (var f in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                  BindingFlags.NonPublic | BindingFlags.Public))
                {
                    switch (f)
                    {
/*                        case FieldInfo fieldInfo:
                            if (!typeof(TChild).IsAssignableFrom(fieldInfo.FieldType)) continue;
                            // Load the instance of the object (argument 0) onto the stack
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Castclass, type);
                            // Load the value of the object's field (fi) onto the stack
                            il.Emit(OpCodes.Ldfld, fieldInfo);

                            il.Emit(OpCodes.Castclass, typeof(TChild));

                            //il.Emit(OpCodes.Stloc,a);
                            il.Emit(OpCodes.Ldarg_0);
                            //il.Emit(OpCodes.Castclass, typeof(TClass));
                            //il.Emit(OpCodes.Ldloc,a);
                            //il.Emit(OpCodes.Call, action.Method);
                            break;*/
                        case PropertyInfo propertyInfo:
                            if (!typeof(TChild).IsAssignableFrom(propertyInfo.PropertyType)) continue;
                            if (propertyInfo.CanWrite) continue;





                            // Load the instance of the object (argument 0) onto the stack
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Castclass, type);
                            // Load the value of the object's field (fi) onto the stack
                            il.Emit(propertyInfo.GetMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
                                propertyInfo.GetMethod);

                            il.Emit(OpCodes.Castclass, typeof(TChild));
                            //il.Emit(OpCodes.Stloc,a);
                            //il.Emit(OpCodes.Pop);
                            //il.Emit(OpCodes.Pop);
                            //il.Emit(OpCodes.Ldloc,a);
                            il.Emit(OpCodes.Callvirt, expression.GetMethodInfo());
                            break;
                    }
                }
                type = type.BaseType;
            }

            il.Emit(OpCodes.Ret);

            return (Action<TClass>)dm.CreateDelegate(typeof(Action<TClass>));
            
        }

    }
}
