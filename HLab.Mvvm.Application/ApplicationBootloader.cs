using System;
using System.Reflection;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Menus;
using HLab.Mvvm.Application.Updater;

namespace HLab.Mvvm.Application
{
    public abstract class ApplicationBootloader(ApplicationBootloader.Injector injector) : IBootloader
    {

        public IUpdater Updater { get; set; }

        // TODO
        //public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }

        public class Injector(
            IMenuService menu, 
            IMvvmService mvvm, 
            IApplicationInfoService info, 
            Func<IApplicationViewModel> getMainViewModel)
        {
            public IMenuService Menu { get; } = menu;
            public IMvvmService Mvvm { get; } = mvvm;
            public IApplicationInfoService Info { get; } = info;
            public Func<IApplicationViewModel> GetMainViewModel { get; } = getMainViewModel;
        }

        public void SetMainViewMode(Type vm)
        {
            MainViewMode = vm;
        }
        public Type MainViewMode { get; private set; }

        public IApplicationViewModel ViewModel { get; set; } 


        static void InitializeCultures()
        {
            // TODO ?
            //FrameworkElement.LanguageProperty.OverrideMetadata(
            //    typeof(FrameworkElement),
            //    new FrameworkPropertyMetadata(
            //        XmlLanguage.GetLanguage(
            //            CultureInfo.CurrentCulture.IetfLanguageTag)));
        }



        public virtual void Load(IBootContext b)
        {
            if (b.WaitDependency("LocalizeBootloader")) return;
            if (b.WaitDependency("LoginBootloader")) return;

            injector.Info.Version = Assembly.GetEntryAssembly()?.GetName().Version;

            InitializeCultures();

            // TODO
            //if (Updater != null )
            //{
            //    Updater.CheckVersion();

            //    if (Updater.NewVersionFound)
            //    {
            //        var updaterView = new ApplicationUpdateView
            //        {
            //            DataContext = Updater
            //        };
            //        // TODO : updaterView.ShowDialog();

            //        if (Updater.Updated)
            //        {
            //            System.Windows.Application.Current.Shutdown();
            //            return;;
            //        }
            //    }
            //}

            ViewModel = injector.GetMainViewModel();

            injector.Menu.RegisterMenu("file", "{File}", null, null);
            injector.Menu.RegisterMenu("data", "{Data}", null, null);
            injector.Menu.RegisterMenu("param", "{Parameters}", null, null);
            injector.Menu.RegisterMenu("tools", "{Tools}", null, null);
            injector.Menu.RegisterMenu("help", "{_?}", null, null);

            injector.Menu.RegisterMenu("file/exit","{Exit}", ViewModel.Exit,null);
        }
    }
}
