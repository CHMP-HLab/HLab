using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application
{
    public abstract class DocumentService : IDocumentService
    {
        [Import] private IMvvmService _mvvm { get; }
        [Import] private Func<Type, object> _getter { get; }

        public abstract Task OpenDocumentAsync(IView content);
        public abstract Task CloseDocumentAsync(object content);

        public object MainViewModel {get;set;}

        public virtual async Task OpenDocumentAsync(object obj)
        {
            if (obj is Type t)
            {
                obj = _getter(t);
            }

            if (obj is IView view)
                await OpenDocumentAsync(view);
            else
            {
                var doc = _mvvm.MainContext.GetView(obj, typeof(ViewModeDefault), typeof(IViewClassDocument));
                await OpenDocumentAsync(doc);
            }
        }
    }
}
