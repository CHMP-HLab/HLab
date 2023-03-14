using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using HLab.Base;
using HLab.Base.Wpf;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.ReactiveUI;
using HLab.Mvvm.Views;
using HLab.Mvvm.Wpf.Views;

namespace HLab.Mvvm.Wpf
{
    public class MvvmServiceWpf : MvvmService
    {
        readonly ResourceDictionary _dictionary = new();

        public MvvmServiceWpf(
            IMessagesService messageBus, 
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
            if (view is not DependencyObject obj) return;

            ViewLocator.SetViewClass(obj,typeof(IViewClassDefault));
            ViewLocator.SetViewMode(obj,typeof(ViewModeDefault));
        }

        static readonly object Template = XamlReader.Parse(@$"
                <DataTemplate 
                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                        <{XamlTool.Type<ViewLocator>(out var ns1)} Model=""{{Binding}}""/>
                </DataTemplate>");

        protected override void Register(Type t)
        {
            if (t.IsInterface) return;

            _dictionary.Add(new DataTemplateKey(t), Template);

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
