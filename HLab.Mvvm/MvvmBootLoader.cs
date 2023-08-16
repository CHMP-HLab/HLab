using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    public class MvvmBootloader(IMvvmService mvvm) : IBootloader
    {
        public void Load(IBootContext b)
        {
            mvvm.Register();
        }
    }
}
