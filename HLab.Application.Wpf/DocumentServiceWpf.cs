using System;
using System.Linq;
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


        public override async Task OpenDocumentAsync(IView content)
        {
            if (MainViewModel is MainWpfViewModel vm)
            {
                if (content is IViewClassAnchorable)
                {
                    if (!vm.Anchorables.Contains(content))
                        vm.Anchorables.Add(content);
                }
                else
                {
                    if (!vm.Documents.Contains(content))
                    {
                        vm.Documents.Add(content);

                        var message = GetMessage(content);

                        MessageBus.Publish(message);
                    }
                }

                vm.ActiveDocument = content as FrameworkElement;
            }
        }

        public override async Task OpenDocumentAsync(object content)
        {

            if (MainViewModel is MainWpfViewModel vm)
            {
                if (content is IView view)
                {
                    if (vm.Documents.Contains(view))
                    {
                        vm.ActiveDocument = view as FrameworkElement;
                        return;
                    }

                    if (vm.Anchorables.Contains(view))
                    {
//                        vm.Anchorables.Remove(view);
                        return;
                    }
                }

                var documents = vm.Documents.OfType<FrameworkElement>().ToList();
                foreach (var document in documents)
                {
                    if (ReferenceEquals(document.DataContext, content))
                    {
                        vm.ActiveDocument = document;
                        return;
                    }

                    else if (document.DataContext is IViewModel mvm && ReferenceEquals(mvm.Model, content))
                    {
                        vm.ActiveDocument = document;
                        return;
                    }
                }

                var anchorables = vm.Anchorables.OfType<FrameworkElement>().ToList();
                foreach (var anchorable in anchorables)
                {
                    if (ReferenceEquals(anchorable.DataContext, content))
                    {
                        return;
                    }
                    else if (anchorable.DataContext is IViewModel mvm && ReferenceEquals(mvm.Model, content))
                    {
                        return;
                    }
                }
            }


            await base.OpenDocumentAsync(content);
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
