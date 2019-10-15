using System;
using System.Threading.Tasks;
using System.Windows;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    [Export(typeof(IMvvmService)), Singleton]
    public class MvvmServiceWpf : MvvmService
    {
        private readonly ResourceDictionary _dictionary = new ResourceDictionary();
        [Import]
        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }
        //[Import]
        //public IApplicationInfoService InfoService { get; set; }
        //public MvvmServiceWpf(IExportLocatorScope scope, Func<Type,object> locate, IMessageBus msg) : base(scope,locate,msg)
        //{
        //}

        private readonly Window _mainWindow = new DefaultWindow();
        public override void SetMainView(IView view)
        {
            _mainWindow.Content = view;
            _mainWindow.DataContext = (view as FrameworkElement)?.DataContext;
            _mainWindow.Show();
        }

        public  void RegisterWithProgress()
        {
            var vm = GetProgressLoadingViewModel();
            //vm.Title = InfoService.Name;

            var progressWindow = new ProgressLoadingView
            {
                DataContext = vm,
            };

            SetMainView(progressWindow);

            var t = Task.Run(() => {
                base.Register();
                Application.Current.Resources.MergedDictionaries.Add(_dictionary);
            });

            t.Wait();

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
