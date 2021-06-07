using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Core.Annotations;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application
{
    public interface IApplicationService : IService
    {
        IApplicationInfoService Info { get; }
        IMvvmService Mvvm { get; }
        IDocumentService Docs { get; }
        IMessageBus Message { get;  }
        IMenuService Menu { get; }
//        INotifyCollectionChanged Countries{ get; }
        ILocalizationService Localization {get;}

    }
}
