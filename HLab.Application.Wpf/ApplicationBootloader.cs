using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.Update;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;

namespace HLab.Mvvm.Application.Wpf
{
    public class ApplicationBootloader : IBootloader
    {
        [Import]
        private IMenuService _menu;

        [Import]
        private IMvvmService _mvvm;

        [Import]
        private IApplicationInfoService _info;

        public IUpdater Updater { get; set; }
        [Import] private readonly Func<MainWpfViewModel> _getVm;

        public void SetMainViewMode(Type vm)
        {
            MainViewMode = vm;
        }
        public Type MainViewMode { get; private set; }

        public MainWpfViewModel ViewModel { get; set; } 
        public Window MainWindow { get; protected set; }
 
        private static void InitializeCultures()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));
        }


        //[Import]
        //public Func<ILoginViewModel> GetLoginViewModel { get; set; }

        [Import]
        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }



        public void Load(IBootContext b)
        {
            if (b.Contains("LocalizeBootloader"))
            {
                b.Requeue();
                return;
            }


            MainWindow = new DefaultWindow()
            {
                //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState =  WindowState.Maximized
            };

            MainWindow.Closing += (sender, args) => System.Windows.Application.Current.Shutdown();
            MainWindow.Show();

            _info.Version = Assembly.GetCallingAssembly().GetName().Version;

            InitializeCultures();

            if (Updater != null )
            {
                Updater.CheckVersion();

                if (Updater.NewVersionFound)
                {
                    var updaterView = new ApplicationUpdateView
                    {
                        DataContext = Updater
                    };
                    // TODO : updaterView.ShowDialog();

                    if (Updater.Updated)
                    {
                        System.Windows.Application.Current.Shutdown();
                        return;;
                    }
                }
            }


            ViewModel = _getVm();
            var w = _mvvm.MainContext.GetView(ViewModel,MainViewMode);


            _menu.RegisterMenu("file", "{File}", null, null);
            _menu.RegisterMenu("data", "{Data}", null, null);
            _menu.RegisterMenu("param", "{Parameters}", null, null);
            _menu.RegisterMenu("tools", "{Tools}", null, null);
            _menu.RegisterMenu("help", "{_?}", null, null);


            _menu.RegisterMenu("file/exit","{Exit}", ViewModel.Exit,null);

            MainWindow.Content = w;

            return;
        }
    }
}
