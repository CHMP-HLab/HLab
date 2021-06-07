using System;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    //public class RuntimeImportContext : IRuntimeImportContext
    //{
    //    public IRuntimeImportContext Parent { get; }

    //    public IImportContext StaticContext { get; private set; }

    //    protected RuntimeImportContext(IRuntimeImportContext parent, IImportContext ctx)
    //    {
    //        Parent = parent;
    //        StaticContext = ctx;
    //        Target = parent?.Target;
    //    }

    //    public object Target { get; private set; }

    //    public static IRuntimeImportContext GetStatic(object target, Type importType) => GetStatic(target,
    //        ImportContext.Create(target?.GetType(), importType));

    //    public static IRuntimeImportContext GetStatic(object target, IImportContext ctx) => new RuntimeImportContext(null,
    //        ctx)
    //    {
    //        Target = target
    //    };

    //    public IRuntimeImportContext NewChild(object target) => new RuntimeImportContext(this,
    //        ImportContext.Create( null, target.GetType() ))
    //    {
    //        Target = target,
    //    };


    //    public IRuntimeImportContext NewChild(object target, IImportContext ctx) => new RuntimeImportContext(this,ctx)
    //    {
    //        Target = target,
    //    };

    //    public T GetTarget<T>()
    //    {
    //        IRuntimeImportContext ctx = this;
    //        while (ctx!=null)
    //        {
    //            if (ctx.Target is T r) return r;
    //            ctx = ctx.Parent;
    //        }

    //        return default(T);
    //    }
    //}
}