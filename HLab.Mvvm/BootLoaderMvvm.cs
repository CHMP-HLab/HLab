using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    class MvvmBootloader : IBootloader
    {
        [Import]
        private readonly IMvvmService _mvvm;

        public bool Load()
        {
            _mvvm.Register();
            return true;
        }
    }
}
