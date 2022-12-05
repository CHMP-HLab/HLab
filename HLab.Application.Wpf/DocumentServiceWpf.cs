using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Application.Wpf
{
    public class DocumentServiceWpf : DocumentService
    {
        public IMessagesService MessageBus { get; }
        Func<object, ISelectedMessage> GetMessage { get; }

        public DocumentServiceWpf(
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
                    FrameworkElement fe => fe.DataContext,
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

                    presenter.ActiveDocument = view as FrameworkElement;
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
                        presenter.RemoveDocument((FrameworkElement)view);
                        return;
                    }

                    if (presenter.Anchorables.Contains(view))
                    {
                        presenter.Anchorables.Remove(view);
                        return;
                    }
                }

                var documents = presenter.Documents.OfType<FrameworkElement>().ToList();
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

                var anchorables = presenter.Anchorables.OfType<FrameworkElement>().ToList();
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
}
