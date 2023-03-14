using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.ReactiveUI;

namespace HLab.Mvvm.Avalonia
{
    public class MvvmAvaloniaImpl : IMvvmPlatformImpl
    {
        readonly ResourceDictionary _dictionary = new();

        public MvvmAvaloniaImpl(
            IMessagesService messageBus, 
            Func<Type, object> locateFunc,
            Func<ProgressLoadingViewModel> getProgressLoadingViewModel
            ) 
        {
            GetProgressLoadingViewModel = getProgressLoadingViewModel;
        }

        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }

        public  void RegisterWithProgress(IMvvmService mvvm)
        {
            var vm = GetProgressLoadingViewModel();
            //vm.Title = InfoService.Name;

            var progressWindow = new ProgressLoadingView
            {
                DataContext = vm,
            };

            progressWindow.AsWindow().Show();
            //SetMainView(progressWindow);

            var t = Task.Run(() => {
                Application.Current.Resources.MergedDictionaries.Add(_dictionary);
            });


            mvvm.ViewHelperFactory.Register<IView>(v=>new ViewHelperAvalonia((StyledElement)v));
        }
        
        public void Register(IMvvmService mvvm)
        {
                Application.Current.Resources.MergedDictionaries.Add(_dictionary);
                mvvm.ViewHelperFactory.Register<IView>(v => new ViewHelperAvalonia((StyledElement)v));
        }

        public void PrepareView(object view)
        {
            if (view is not AvaloniaObject obj) return;

            ViewLocator.SetViewClass(obj,typeof(IDefaultViewClass));
            ViewLocator.SetViewMode(obj,typeof(DefaultViewMode));
        }

        public void Register(Type t)
        {
            if (t.IsInterface) return;

            var template = new FuncDataTemplate(t,(value, namescope) =>
                new ViewLocator());


//            Application.Current.Dispatcher.InvokeAsync(()=>
//TODO : Avalonia not sure about the key
            _dictionary.Add(t, template)
//                )
                ;

        }

        public IView GetNotFoundView(Type viewModelType, Type viewMode, Type viewClass)
        {
            return new NotFoundView
            {
                Title = { Content = "View not found" },
                Message = { Content = (viewModelType?.ToString() ?? "??") 
                                      + "\n" + (viewMode?.FullName ?? "??") 
                                      + "\n" + (viewClass?.FullName ?? "??") }
            };
        }

    }
}
