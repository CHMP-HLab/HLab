using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons.Providers;

namespace HLab.Icon.Benchmark
{



    public class IconBenchmark
    {
        readonly ResourceManager _resourceManager = new(Assembly.GetExecutingAssembly().GetName().Name + ".g", Assembly.GetExecutingAssembly());

        //readonly IIconProvider _providerUri = new IconProviderXamlFromUri(new Uri(
        //        "/" + Assembly.GetExecutingAssembly().FullName + ";component/location.xaml", UriKind.Relative));

        IIconProvider _providerResourceXaml;
        IIconProvider _providerSourceXaml;
        IIconProvider _providerUriXaml;
        IIconProvider _providerResourceSvg;
        IIconProvider _providerSourceSvg;
        readonly Window _window = new Window();

        public IconBenchmark()
        {
            _window.Background = Brushes.Gray;
            _window.Show();
        }

        [GlobalSetup]
        public void Setup()
        {
            _providerResourceXaml = new IconProviderXamlFromResource(_resourceManager,"Icons/location.rsc.xaml",Colors.Black);
            _providerSourceXaml = new IconProviderXamlFromResource(_resourceManager, "Icons/location.rsc.xaml", Colors.Black);
            var e = _providerSourceXaml.Get();

            _providerResourceSvg = new IconProviderXamlFromResource(_resourceManager, "Icons/location.rsc.svg", Colors.Black);
            _providerSourceSvg = new IconProviderXamlFromResource(_resourceManager, "Icons/location.rsc.svg", Colors.Black);
            var f = _providerSourceXaml.Get();

            _providerUriXaml = new IconProviderXamlFromUri(
                new Uri("/" + Assembly.GetExecutingAssembly().FullName + ";component/Icons/location.xaml", UriKind.Relative));

        }

        public void GlobalCleanup()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));
        }


        [Benchmark, STAThread]
        public object Xaml()
        {
            var xaml = new LocationWithCode();
            _window.Content = xaml;
            GlobalCleanup();
            return xaml;
        }

        [Benchmark(Baseline = true), STAThread]
        public object ResourceXaml()
        {
            var xaml = _providerResourceXaml.Get();
            _window.Content = xaml;GlobalCleanup();
            return xaml;
        }
        [Benchmark, STAThread]
        public object SourceXaml()
        {
            var xaml = _providerSourceXaml.Get();
            _window.Content = xaml;GlobalCleanup();
            return xaml;
        }
        [Benchmark, STAThread]
        public object Uri()
        {
            var xaml = _providerUriXaml.Get();
            _window.Content = xaml;GlobalCleanup();
            return xaml;
        }
        [Benchmark, STAThread]
        public object ResourceSvg()
        {
            var xaml = _providerResourceSvg.Get();
            _window.Content = xaml;GlobalCleanup();
            return xaml;
        }
        [Benchmark, STAThread]
        public object SourceSvg()
        {
            var xaml = _providerSourceSvg.Get();
            _window.Content = xaml;GlobalCleanup();
            return xaml;
        }
    }


    internal class Program
    {
        static readonly  ResourceManager ResourceManager = new(Assembly.GetExecutingAssembly().GetName().Name +".g", Assembly.GetExecutingAssembly());
        
        [STAThread]
        static void Main(string[] args)
        {
            var names = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var resources = ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            foreach (DictionaryEntry entry in resources)
            {
                
            }
            //ConsoleManager.Show();
            var w = new MainWindow();
            //var cw = new TextBlockWriter(w);
            var cw = new Writer();

            Console.SetOut(cw);
            
            //w.Do(()=>
            //{
            //    BenchmarkRunner.Run<IconBenchmark>();
            //    Console.WriteLine("...End");
            //});

            BenchmarkRunner.Run<IconBenchmark>();

            w.TextBlock.Text = cw.Text;
            w.ShowDialog();
        }
    }

    public class Writer : TextWriter
    {
        public string Text = "";
        public Writer()
        {
        }

        public override void Write(char value)
        {
            Text += value;
        }

        public override void Write(string value)
        {
            Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
    public class TextBlockWriter : TextWriter
    {
        readonly MainWindow _window;
        public string Text = "";
        public TextBlockWriter(MainWindow w)
        {
            _window = w;
        }

        public override void Write(char value)
        {
            _window.Dispatcher.Invoke(
                () =>
                {
                    _window.TextBlock.Text += value;
                    _window.ScrollViewer.ScrollToEnd();
                },DispatcherPriority.Render
                
                );
            Text += value;
        }

        public override void Write(string value)
        {
            _window.Dispatcher.Invoke(() =>
            {
                _window.TextBlock.Text += value;
                _window.ScrollViewer.ScrollToEnd();
            },DispatcherPriority.Render);
            Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
