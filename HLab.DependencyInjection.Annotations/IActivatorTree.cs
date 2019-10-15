namespace HLab.DependencyInjection.Annotations
{
    public interface IActivatorTree
    {
        IImportContext Context { get; set; }
        IActivatorKey Key { get; set; }
        IActivatorTree Parent { get; }

        IActivatorTree GetRecursive();

#if DEBUG
        string ToString();
        string ReadableTree();
        string TabbedBranch();
#endif
    }
}