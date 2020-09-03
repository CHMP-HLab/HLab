namespace HLab.Erp.Core
{
    public interface IUpdater
    {
        void Update();
        void CheckVersion();
        bool NewVersionFound { get; }
        bool Updated { get; }
    }
}
