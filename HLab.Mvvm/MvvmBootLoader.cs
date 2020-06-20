using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    class MvvmBootloader : IBootloader
    {
        [Import]
        private readonly IMvvmService _mvvm;

        public void Load(IBootContext b)
        {
            _mvvm.Register();
        }
    }
}
