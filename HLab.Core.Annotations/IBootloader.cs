using System;

namespace HLab.Core.Annotations
{
    // TODO : Progress for bootloader



    public interface IBootContext
    {
        void Requeue();
        bool StillContains(params string[] name);

        bool StillContainsAndRequeue(params string[] name)
        {
            var contains = StillContains(name);
            if (contains) Requeue();
            return contains;
        }

        bool StillContainsAndRequeue<T>() => StillContainsAndRequeue(typeof(T).Name);
    }

    public interface IBootloader
    {
        void Load(IBootContext bootstrapper);
    }

}
