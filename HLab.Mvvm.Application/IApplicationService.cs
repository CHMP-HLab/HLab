using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Menus;

namespace HLab.Mvvm.Application;

public interface IApplicationService : IService
{
    IApplicationInfoService Info { get; }
    IMvvmService Mvvm { get; }
    IDocumentService Docs { get; }
    IMessagesService Message { get;  }
    IMenuService Menu { get; }
//        INotifyCollectionChanged Countries{ get; }
    ILocalizationService Localization {get;}

}
