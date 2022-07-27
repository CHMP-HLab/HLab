using System;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application
{
    public abstract class DocumentService : IDocumentService
    {
        readonly IMvvmService _mvvm;
        readonly Func<Type, object> _getter;

        protected DocumentService(IMvvmService mvvm, Func<Type, object> getter)
        {
            _mvvm = mvvm;
            _getter = getter;
        }

        public abstract Task OpenDocumentAsync(IView content);
        public virtual Task OpenDocumentAsync<T>() => OpenDocumentAsync(typeof(T));

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
