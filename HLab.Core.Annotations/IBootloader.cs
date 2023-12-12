using System.Threading.Tasks;

namespace HLab.Core.Annotations;
// TODO : Progress for bootloader

public interface IBootContext
{
    void Requeue();
    bool StillContains(params string[] name);

    bool WaitDependency(params string[] name)
    {
        var contains = StillContains(name);
        if (contains) Requeue();
        return contains;
    }
    bool WaitingForService(IService service)
    {
        if (service.ServiceState != ServiceState.NotConfigured) return false;

        Requeue();
        return true;
    }

    bool WaitDependency<T>() => WaitDependency(typeof(T).Name);
}

public interface IBootloader
{
    Task LoadAsync(IBootContext bootstrapper);
}
/*

public class MyBootloader : IBootloader
{
    public MyBootloader() // <- Inject here
    {
    }

    public void Load(IBootContext b)
    {
    }

}

*/