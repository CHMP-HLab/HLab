using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Mvvm.Avalonia;

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

    public Task PrepareViewAsync(IView view, CancellationToken token)
    {
        if (view is not AvaloniaObject obj) throw new InvalidCastException("IView objects should be AvaloniaObject in Avalonia implementation");


        if (Dispatcher.UIThread.CheckAccess())
        {
            ViewLocator.SetViewClass(obj,typeof(IDefaultViewClass));
            ViewLocator.SetViewMode(obj,typeof(DefaultViewMode));

            LinkDispose(view);
            
            return Task.CompletedTask;
        }

        return Dispatcher.UIThread.InvokeAsync(() =>
        {
            ViewLocator.SetViewClass(obj,typeof(IDefaultViewClass));
            ViewLocator.SetViewMode(obj,typeof(DefaultViewMode));

            LinkDispose(view);

        }, DispatcherPriority.Default, token).GetTask();

        //TODO Check if this is still needed, was a try to fix memory leak
        void LinkDispose(IView v)
        {
            if (v is not StyledElement element) return;
            element.DetachedFromLogicalTree += (a,o) =>
            {
                if (element.DataContext is IDisposable vm)
                {
                    vm.Dispose();
                }
                // Dispatcher.UIThread.RunJobs();
                GC.Collect();
            };
        }
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

    public async Task<IView> GetNotFoundViewAsync(Type viewModelType, Type viewMode, Type viewClass, CancellationToken token = default)
    {
        return await Dispatcher.UIThread.InvokeAsync(() => new NotFoundView
            {
                Title = { Content = "View not found" },
                Message = { Content = (viewModelType?.ToString() ?? "??") 
                                      + "\n" + (viewMode?.FullName ?? "??") 
                                      + "\n" + (viewClass?.FullName ?? "??") }
            }
            , DispatcherPriority.Normal
            , token
        );

    }

    public object Activate(IView obj)
    {
        if(obj is IActivatableViewModel a) a.Activator.Activate();

        return obj;
    }

    public object Deactivate(IView obj)
    {
        if(obj is IActivatableViewModel a) a.Activator.Deactivate();
        throw new NotImplementedException();
    }
}