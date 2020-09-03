using System;

namespace HLab.Core.Annotations
{
    // TODO : Progress for bootloader


    public static class BootstrapperExtension
    {
        public static void Enqueue(this IBootContext context, IBootloader bootloader)
        {
            context.Enqueue(bootloader.GetType().Name,bootloader.Load);
        }
    }

    public interface IBootContext
    {
        void Requeue();
        void Enqueue(string name,Action<IBootContext> action);
        bool Contains(string name);
    }

    public interface IBootloader
    {
        void Load(IBootContext bootstrapper);
    }

    public interface IBootloaderDependent : IBootloader
    {
        string[] DependsOn {get;}
    }
}
