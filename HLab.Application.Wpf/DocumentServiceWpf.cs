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

        public override async Task OpenDocumentAsync(IView view)
        {
            if (MainViewModel is MainWpfViewModel vm)
            {
                if (view is IViewClassAnchorable)
                {
                    if (!vm.Anchorables.Contains(view))
                        vm.Anchorables.Add(view);
                }
                else
                {
                    if (!vm.Documents.Contains(view))
                    {
                        vm.Documents.Add(view);

                        MessageBus.Publish(GetMessage(view));
                    }

                    vm.ActiveDocument = view as FrameworkElement;
                }


                var model = GetModel(view);
                var documents = vm.Documents.ToList();
                foreach (var document in documents)
                {
                    var documentModel = GetModel(document);

                    if (!ReferenceEquals(model, documentModel)) continue;

                    vm.ActiveDocument = document;
                    return;
                }

                var anchorables = vm.Anchorables.ToList();
                foreach (var anchorable in anchorables)
                {
                    var documentModel = GetModel(anchorable);
                    if (ReferenceEquals(model, documentModel))
                    {
                        return;
                    }
                }

            }
        }


        public override async Task CloseDocumentAsync(object content)
        {
            if (MainViewModel is MainWpfViewModel vm)
            {
                if (content is IView view)
                {
                    if (vm.Documents.Contains(view))
                    {
                        vm.RemoveDocument((FrameworkElement)view);
                        return;
                    }

                    if (vm.Anchorables.Contains(view))
                    {
                        vm.Anchorables.Remove(view);
                        return;
                    }
                }

                var documents = vm.Documents.OfType<FrameworkElement>().ToList();
                foreach (var document in documents)
                {
                    if (ReferenceEquals(document.DataContext, content))
                    {
                        vm.RemoveDocument(document);
                    }

                    else if (document.DataContext is IViewModel mvm && ReferenceEquals(mvm.Model, content))
                    {
                        vm.RemoveDocument(document);
                    }
                }

                var anchorables = vm.Anchorables.OfType<FrameworkElement>().ToList();
                foreach (var anchorable in anchorables)
                {
                    if (ReferenceEquals(anchorable.DataContext, content))
                    {
                        vm.Anchorables.Remove(anchorable);
                    }
                    else if (anchorable.DataContext is IViewModel mvm && ReferenceEquals(mvm.Model, content))
                    {
                        vm.Anchorables.Remove(anchorable);
                    }
                }

            }
        }
    }
}
