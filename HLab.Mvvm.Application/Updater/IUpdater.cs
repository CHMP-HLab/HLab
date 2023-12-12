namespace HLab.Mvvm.Application.Updater;

public interface IUpdater
{
    void Update();
    void CheckVersion();
    bool NewVersionFound { get; }
    bool Updated { get; }
}
