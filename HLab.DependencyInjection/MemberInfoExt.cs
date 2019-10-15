using System;
using System.Reflection;

namespace HLab.DependencyInjection
{
    public static class MemberInfoExt
    {
        public static Type GetReturnType(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo fi:
                    return fi.FieldType;
                case PropertyInfo pi:
                    return pi.PropertyType;
                case MethodInfo mi:
                    return mi.ReturnType;
                default:
                    return null;
            }
        }
    }
}
