﻿using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

using HLab.Core.Annotations;
using HLab.Erp.Acl.LoginServices;
using HLab.Erp.Core;
using HLab.Erp.Core.Update;
using HLab.Erp.Core.Wpf.Localization;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Views;

namespace HLab.Mvvm.Application.Wpf
{
    public class ApplicationBootloader : IBootloader
    {
        private readonly IMenuService _menu;

        private readonly IMvvmService _mvvm;

        private readonly IApplicationInfoService _info;

        public IUpdater Updater { get; set; }
        private readonly Func<MainWpfViewModel> _getVm;

        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }

        public ApplicationBootloader(IMenuService menu, IMvvmService mvvm, IApplicationInfoService info, Func<MainWpfViewModel> getVm)
        {
            _menu = menu;
            _mvvm = mvvm;
            _info = info;
            _getVm = getVm;
        }

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



        public void Load(IBootContext b)
        {
            if (b.WaitDependency<LocalizeBootloader>()) return;
            if (b.WaitDependency<LoginBootloader>()) return;

            _info.Version = Assembly.GetEntryAssembly()?.GetName().Version;

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

            MainWindow = _mvvm.MainContext.GetView(ViewModel,MainViewMode).AsWindow();
            MainWindow.Closing += (sender, args) => System.Windows.Application.Current.Shutdown();

            _menu.RegisterMenu("file", "{File}", null, null);
            _menu.RegisterMenu("data", "{Data}", null, null);
            _menu.RegisterMenu("param", "{Parameters}", null, null);
            _menu.RegisterMenu("tools", "{Tools}", null, null);
            _menu.RegisterMenu("help", "{_?}", null, null);


            _menu.RegisterMenu("file/exit","{Exit}", ViewModel.Exit,null);

            MainWindow.Show();

            return;
        }
    }
}
