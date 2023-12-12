using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm;

public class MvvmBootloader(IMvvmService mvvm) : IBootloader
{
    public async Task LoadAsync(IBootContext b)
    {
        await mvvm.RegisterAsync();
    }
}