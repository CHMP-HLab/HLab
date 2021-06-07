using System;
using System.Linq;
using System.Reflection;

namespace HLab.DependencyInjection.Annotations
{
    public static class ExportWhenExtension
    {
        public static IConfigurator InProperty(this IConfigurator c)
            => c.When(t => t.Context.TargetMemberInfo is PropertyInfo);
        public static IConfigurator InField(this IConfigurator c)
            => c.When(t => t.Context.TargetMemberInfo is FieldInfo);
        public static IConfigurator InMethod(this IConfigurator c)
            => c.When(t => t.Context.TargetMemberInfo is MethodInfo);
        public static IConfigurator InConstructor(this IConfigurator c)
            => c.When(t => t.Context.TargetMemberInfo is ConstructorInfo);
    }

    public static class ExportEntryExt
    {
        public static IConfigurator AutoExport<T>(this IConfigurator c)
            => c.NewEntry()
                .ExportType(t => t.Context.TargetType)
                .When(t => typeof(T).IsAssignableFrom(t.Context.TargetType))
                .As<T>();

        public static IConfigurator Set(this IConfigurator c, Action<IExportEntry> action)
        {
            action(c.LastEntry);
            return c;
        }

        public static IConfigurator ExportType(this IConfigurator c, Func<IActivatorTree, Type> exportType)
            => c.Set(e => e.ExportType = exportType);

        public static IConfigurator Export<T>(this IConfigurator c) => c.NewEntry().ExportType(ctx => typeof(T));
        public static IConfigurator Export(this IConfigurator c, Type exportType)
        {
            if (exportType.IsGenericType && !exportType.IsConstructedGenericType)
                return c.NewEntry().ExportType(tree =>
                {
                    if (tree == null || !tree.Context.ImportType.IsGenericType) return exportType;
                    //Debug.Assert(ctx.ImportType.IsGenericType && ctx.ImportType.IsConstructedGenericType);
                    //TODO : better check for argument compatibility
                    var et = exportType.GenericReadableName();
                    var nb2 = tree.Context.ImportType.GenericReadableName();
                    return exportType.MakeGenericType(tree.Context.ImportType.GetGenericArguments());
                });

            return c.NewEntry().ExportType(ctx => exportType);
        }

        public static IConfigurator As<T>(this IConfigurator c) => c.When(t => t.Context.ImportType == typeof(T));
        public static IConfigurator As(this IConfigurator c, Type importType)
        {
            if (importType.IsGenericType && !importType.IsConstructedGenericType)
                return c.OrWhen(t => t.Context.ImportType.IsGenericType && t.Context.ImportType.GetGenericTypeDefinition() == importType);

            return c.OrWhen(t =>
            {
                var result =  t.Context.ImportType == importType;
                #if DEBUG
                if(!result && t.Context.ImportType.AssemblyQualifiedName == importType.AssemblyQualifiedName) 
                    throw new Exception("Type exists twice");
                #endif
                return result;
            });
        }
        public static IConfigurator AsAnnotated(this IConfigurator c)
        {
            var exportType = c.LastEntry.ExportType(null);

            if (exportType == null) throw new Exception("Export type must be known");
            foreach (var attribute in exportType.GetCustomAttributes<ExportAttribute>())
            {
                c = attribute.Configurator(c);
            }

            return c;
        }
        public static IConfigurator SetMode(this IConfigurator c, ExportMode mode)
        {
            c.Entries.Last().AddMode(mode);
            return c;
        }

        public static IConfigurator Singleton(this IConfigurator c)
        {
            return c
                .Set(e  => e.SetSingletonLocator())
                //.SetMode(ExportMode.Singleton)
                ;
        }

        public static IConfigurator Decorator(this IConfigurator c)
        {
            c.SetMode(ExportMode.Decorator);
            return c;
        }
        public static IConfigurator When(this IConfigurator c, Func<IActivatorTree, bool> condition)
        {
            c.LastEntry.AndCondition(condition);
            return c;
        }
        public static IConfigurator OrWhen(this IConfigurator c, Func<IActivatorTree, bool> condition)
        {
            c.LastEntry.OrCondition(condition);
            return c;
        }
        public static IConfigurator WhenInjectedInto<T>(this IConfigurator c) => c.When(t => typeof(T).IsAssignableFrom(t.Context.TargetType));

        public static IConfigurator WhenInjectedInto(this IConfigurator c, Type targetType) => c.When(t => targetType.IsAssignableFrom(t.Context.TargetType));

        public static IConfigurator GenericAsTarget(this IConfigurator c)
        {
            var getExport = c.LastEntry.ExportType;
            c.LastEntry.ExportType = t =>
            {
                var type = getExport(t);
                if (type.IsGenericTypeDefinition)
                {
                    return type.MakeGenericType(t.Context.TargetType);
                }
                return type;
            };
            return c;
        }

        public static IConfigurator ExportGenericAsTargetCovariant(
            this IConfigurator c, Type targetType, Type exportType) => c.NewEntry().ExportType(t =>
            {
                var iface = t.Context.TargetType.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == targetType);
                if (iface != null)
                {
                    var type = iface.GetGenericArguments()[0];
                    return exportType.MakeGenericType(type);
                }
                throw new Exception(targetType.Name + " interface not found");
            });

        public static IConfigurator WithPriority(this IConfigurator c, int priority)
        {
            c.Entries.Last().SetPriority(priority);
            return c;
        }

        public static Type[] GetTypes(this object[] parameters)
        {
            var types = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].GetType();
            }

            return types;
        }

        public static string GenericReadableName(this Type t)
        {
            if(t==null) return "[none]";
            if (!t.IsGenericType)
                return t.Name;
            var genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            var genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(GenericReadableName).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }
    }
}
