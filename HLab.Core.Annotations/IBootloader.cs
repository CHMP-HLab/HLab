namespace HLab.Core.Annotations
{
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
        bool WaitService(IService service)
        {
            if(service.ServiceState == ServiceState.NotConfigured)
            {
                Requeue();
                return true;
            }
            return false;
        }

        bool WaitDependency<T>() => WaitDependency(typeof(T).Name);
    }

    public interface IBootloader
    {
        void Load(IBootContext bootstrapper);
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
}
