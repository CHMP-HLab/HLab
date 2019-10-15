namespace HLab.DependencyInjection.Annotations
{
    public interface IRuntimeImportContext
    {
        IRuntimeImportContext Parent { get; }
        IImportContext StaticContext { get; }
        object Target { get; }

        IRuntimeImportContext Get(object target);
        IRuntimeImportContext Get(object target, IImportContext ctx);
        T GetTarget<T>();
    }
}