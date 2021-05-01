using Grace.DependencyInjection.Attributes;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    public class MvvmBootloader : IBootloader
    {
        private readonly IMvvmService _mvvm;

        public MvvmBootloader(IMvvmService mvvm)
        {
            _mvvm = mvvm;
        }

        public void Load(IBootContext b)
        {
            _mvvm.Register();
        }
    }
}
