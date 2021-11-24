﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock.Layout.Serialization;
using HLab.Erp.Acl.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Options;

namespace HLab.Mvvm.Application.Wpf
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
        , IView<MainWpfViewModel>
        , IView<ViewModeKiosk, MainWpfViewModel>
    {
        private const string LayoutFileName = "layout.xml";

        public MainWindow(IOptionsService options/*, IDragDropService drag*/)
        {
            _options = options;
            InitializeComponent();

            //LoadLayout(); // Hang on printing forms

            Loaded += MainWindow_Loaded;
            DataContextChanged += MainWindow_DataContextChanged;

            //drag.RegisterDragCanvas(DragCanvas);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this);
            if (w != null)
            {
                w.Closing += W_Closing;

                w.Top = _options.GetValue("Display","Top",null,()=>0.0,"registry");
                w.Left = _options.GetValue("Display","Left",null,()=>0.0,"registry");
                w.Width = _options.GetValue("Display","Width",null,()=>SystemParameters.PrimaryScreenWidth,"registry");
                w.Height = _options.GetValue("Display","Height",null,()=>SystemParameters.PrimaryScreenHeight,"registry");

                w.WindowState = _options.GetValue("Display","WindowState",null,()=>WindowState.Maximized,"registry");

            }
        }

        private void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var w = Window.GetWindow(this);
            _options.SetValue("Display","Top",w.Top,"registry");
            _options.SetValue("Display","Left",w.Left,"registry");
            _options.SetValue("Display","Width",w.Width,"registry");
            _options.SetValue("Display","Height",w.Height,"registry");

            _options.SetValue("Display","WindowState",w.WindowState,"registry");

            SaveLayout();
        }

        private readonly IOptionsService _options;

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ctx = this.GetValue(ViewLocator.MvvmContextProperty);
        }

        private int? UserId => null; // TODO _acl.Connection.UserId

        private void SaveLayout()
        {
            var layoutSerializer = new XmlLayoutSerializer(DockingManager);
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            layoutSerializer.Serialize(writer);
            _options.SetValue<string>("","Layout", sb.ToString(),"", UserId);
        }

        private async void LoadLayout()
        {
            var layoutSerializer = new XmlLayoutSerializer(DockingManager);

            try
            {
                var layout = await _options.GetValueAsync<string>("","Layout",null,null,UserId).ConfigureAwait(true);
                if (!string.IsNullOrWhiteSpace(layout))
                {
                    using var reader = new StringReader(layout);
                    layoutSerializer.Deserialize(reader);
                }
            }
            catch (FileNotFoundException ex)
            {
                var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (DirectoryNotFoundException ex)
            {
                var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                var res = MessageBox.Show(ex.Message, "Debug", MessageBoxButton.OK,
                    MessageBoxImage.Information);                
            }

        }
    }
}
