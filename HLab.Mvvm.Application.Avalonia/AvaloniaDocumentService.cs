using Avalonia.Controls;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Messages;

namespace HLab.Mvvm.Application.Avalonia;

public class AvaloniaDocumentService : DocumentService
{
    public IMessagesService MessageBus { get; }
    Func<object, ISelectedMessage> GetMessage { get; }

    public AvaloniaDocumentService(
        IMvvmService mvvm,
        Func<Type, object> getter,
        IMessagesService messageBus,
        Func<object, ISelectedMessage> getMessage
    ) : base(mvvm,getter)
    {
        MessageBus = messageBus;
        GetMessage = getMessage;
    }

    object GetModel(object view)
    {
        var o = view;
        while (true)
        {
            var linked = o switch
            {
                Control c => c.DataContext,
                IViewModel vm => vm.Model,
                _ => null
            };

            if (linked is null) return o;
            o = linked;
        }
    }

    public override async Task OpenDocumentAsync(IView view, IDocumentPresenter presenter)
    {
        if (view is IViewClassAnchorable)
        {
            if (!presenter.Anchorables.Contains(view))
                presenter.Anchorables.Add(view);
        }
        else
        {
            if (!presenter.Documents.Contains(view))
            {
                presenter.Documents.Add(view);

                MessageBus.Publish(GetMessage(view));
            }

            presenter.ActiveDocument = view as Control;
        }


        var model = GetModel(view);
        var documents = presenter.Documents.ToList();
        foreach (var document in documents)
        {
            var documentModel = GetModel(document);

            if (!ReferenceEquals(model, documentModel)) continue;

            presenter.ActiveDocument = document;
            return;
        }

        var anchorables = presenter.Anchorables.ToList();
        foreach (var anchorable in anchorables)
        {
            var documentModel = GetModel(anchorable);
            if (ReferenceEquals(model, documentModel))
            {
                return;
            }
        }

    }


    public override async Task CloseDocumentAsync(object content, IDocumentPresenter presenter)
    {
        if (content is IView view)
        {
            if (presenter.Documents.Contains(view))
            {
                presenter.RemoveDocument((Control)view);
                return;
            }

            if (presenter.Anchorables.Contains(view))
            {
                presenter.Anchorables.Remove(view);
                return;
            }
        }

        var documents = presenter.Documents.OfType<Control>().ToList();
        foreach (var document in documents)
        {
            if (ReferenceEquals(document.DataContext, content))
            {
                presenter.RemoveDocument(document);
            }

            else if (document.DataContext is IViewModel mvm && ReferenceEquals(mvm.Model, content))
            {
                presenter.RemoveDocument(document);
            }
        }

        var anchorables = presenter.Anchorables.OfType<Control>().ToList();
        foreach (var anchorable in anchorables)
        {
            if (ReferenceEquals(anchorable.DataContext, content))
            {
                presenter.Anchorables.Remove(anchorable);
            }
            else if (anchorable.DataContext is IViewModel mvm && ReferenceEquals(mvm.Model, content))
            {
                presenter.Anchorables.Remove(anchorable);
            }
        }

    }
}