using System;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;

namespace HLab.Mvvm
{
    public class MvvmServiceWpf : MvvmService
    {
        private readonly ResourceDictionary _dictionary = new();

        public MvvmServiceWpf(
            IMessageBus messageBus, 
            Func<Type, object> locateFunc,
            Func<ProgressLoadingViewModel> getProgressLoadingViewModel
            ) : base(messageBus, locateFunc)
        {
            GetProgressLoadingViewModel = getProgressLoadingViewModel;
        }

        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }

        public  void RegisterWithProgress()
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
                base.Register();
                Application.Current.Resources.MergedDictionaries.Add(_dictionary);
            });


            ViewHelperFactory.Register<IView>(v=>new ViewHelperWpf((FrameworkElement)v));
        }
        
        public override void Register()
        {
                base.Register();
                Application.Current.Resources.MergedDictionaries.Add(_dictionary);
            ViewHelperFactory.Register<IView>(v => new ViewHelperWpf((FrameworkElement)v));
        }

        public override void PrepareView(object view)
        {
            if (view is DependencyObject obj)
            {
                ViewLocator.SetViewClass(obj,typeof(IViewClassDefault));
                ViewLocator.SetViewMode(obj,typeof(ViewModeDefault));
            }
        }

        protected override void Register(Type t)
        {
            if (t.IsInterface) return;

            var viewLocatorFactory = new FrameworkElementFactory(typeof(ViewLocator));
            var template = new DataTemplate(t)
            {
                VisualTree = viewLocatorFactory
            };

//            Application.Current.Dispatcher.InvokeAsync(()=>
            _dictionary.Add(new DataTemplateKey(t), template)
//                )
                ;

        }

        public override IView GetNotFoundView(Type viewModelType, Type viewMode, Type viewClass)
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
