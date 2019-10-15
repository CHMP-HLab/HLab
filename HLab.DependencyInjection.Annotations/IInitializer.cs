namespace HLab.DependencyInjection.Annotations
{
    public interface IInitializer
    {
        void Initialize(IRuntimeImportContext ctx, object[] args);
    }
}
