using System;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application.Documents;

public abstract class DocumentService : IDocumentService
{
    readonly IMvvmService _mvvm;
    readonly Func<Type, object> _getter;

    protected DocumentService(IMvvmService mvvm, Func<Type, object> getter)
    {
        _mvvm = mvvm;
        _getter = getter;
    }

    public abstract Task OpenDocumentAsync(IView content, IDocumentPresenter presenter);

    public abstract Task CloseDocumentAsync(object content, IDocumentPresenter presenter);

    public IDocumentPresenter MainPresenter { get; set; }

    public async Task OpenDocumentAsync(object obj, IDocumentPresenter presenter)
    {
        if (obj is Type t)
        {
            obj = _getter(t);
        }

        if (obj is IView view)
            await OpenDocumentAsync(view, presenter);
        else
        {
            var doc = await _mvvm.MainContext.GetViewAsync(obj, typeof(DefaultViewMode), typeof(IViewClassDocument));
            await OpenDocumentAsync(doc, presenter);
        }
    }
}
