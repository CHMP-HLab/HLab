using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia
{
    public class MvvmServiceAvalonia : MvvmService
    {
        readonly ResourceDictionary _dictionary = new();

        public MvvmServiceAvalonia(
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


            ViewHelperFactory.Register<IView>(v=>new ViewHelperAvalonia((IStyledElement)v));
        }
        
        public override void Register()
        {
                base.Register();
                Application.Current.Resources.MergedDictionaries.Add(_dictionary);
            ViewHelperFactory.Register<IView>(v => new ViewHelperAvalonia((IStyledElement)v));
        }

        public override void PrepareView(object view)
        {
            if (view is AvaloniaObject obj)
            {
                ViewLocator.SetViewClass(obj,typeof(IViewClassDefault));
                ViewLocator.SetViewMode(obj,typeof(ViewModeDefault));
            }
        }

        protected override void Register(Type t)
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
