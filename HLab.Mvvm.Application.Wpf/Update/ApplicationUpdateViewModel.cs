using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Core;
using HLab.Erp.Core.Update;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Application.Wpf.Update
{
    using H = H<UpdaterWpf>;

    public class UpdaterWpf : NotifierBase, IUpdater
    {
        readonly IApplicationInfoService _info;
        public UpdaterWpf(IApplicationInfoService info)
        {
            _info = info;
            H.Initialize(this);
        }


        public string Message
        {
            get => _message.Get();
            set => _message.Set(value);
        }

        readonly IProperty<string> _message = H.Property<string>();

        public String FileName
        {
            get => _fileName.Get();
            set => _fileName.Set(value);
        }

        readonly IProperty<string> _fileName = H.Property<string>();


        // http://www.chmp.org/sites/default/files/apps/sampling/
        public String Url
        {
            get => _url.Get();
            set => _url.Set(value);
        }

        readonly IProperty<string> _url = H.Property<string>();

        public Version NewVersion
        {
            get => _newVersion.Get();
            set => _newVersion.Set(value);
        }

        readonly IProperty<Version> _newVersion = H.Property<Version>();

        public double Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }

        readonly IProperty<double> _progress = H.Property<double>();

        public bool Updated
        {
            get => _updated.Get();
            set => _updated.Set(value);
        }

        readonly IProperty<bool> _updated = H.Property<bool>(c => c.Default(false));

        public void Update()
        {
            var filename = FileName.Replace("{version}", NewVersion.ToString());
            var path = Path.GetTempPath() + filename;

            var task = Task.Run(() => {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Url + filename), path);
            });
        }

        void RunUpdate()
        {
            var filename = FileName.Replace("{version}", NewVersion.ToString());
            var path = Path.GetTempPath() + filename;
            var startInfo = new ProcessStartInfo(path) { Verb = "runas" };
            try
            {
                Process.Start(startInfo);
                Updated = true;
            }
            catch (Win32Exception)
            {
                Message = "L'execution a échouée";
            }
            catch (WebException)
            {
                Message = "Le téléchargement a échoué";
            }
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var bytesIn = double.Parse(e.BytesReceived.ToString());
            var totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            Progress = bytesIn / totalBytes * 100;
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            RunUpdate();
        }
        public void CheckVersion()
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(Url + "version");

                request.Method = "GET";

                var response = request.GetResponse() as HttpWebResponse;

                var streamResponse = response?.GetResponseStream();
                if (streamResponse == null) return;

                var streamRead = new StreamReader(streamResponse);

                var version = Version.Parse(streamRead.ReadToEnd());

                streamResponse.Close();
                streamRead.Close();
                response.Close();

                NewVersion = version;
            }
            catch (UriFormatException e)
            {
                Message = e.Message;
            }
            catch (WebException e)
            {
                Message = e.Message;
            }
            catch (ArgumentException e)
            {
                Message = e.Message;
            }
        }

        public bool NewVersionFound => _newVersionFound.Get();

        readonly IProperty<bool> _newVersionFound = H.Property<bool>(c => c
            .On(e => e.NewVersion)
            .On(e => e.CurrentVersion)
            .Set(e => e.NewVersion > e.CurrentVersion)
        );


        public Version CurrentVersion => _currentVersion.Get();

        readonly IProperty<Version> _currentVersion = H.Property<Version>(c => c
            .Set(e => e._info.Version)
        );

    }

    public class ApplicationUpdateViewModel : ViewModel<UpdaterWpf>
    {
        public ApplicationUpdateViewModel() => H<ApplicationUpdateViewModel>.Initialize(this);

        public void Show()
        {
            var view = new ApplicationUpdateView
            {
                DataContext = this
            };
            // TODO : view.ShowDialog();
        }


        public ICommand UpdateCommand { get; } = H<ApplicationUpdateViewModel>.Command( c => c
            .CanExecute(e=>e.Model.NewVersionFound)
            .Action(e => e.Model.Update())
            .On(e => e.Model.NewVersionFound).CheckCanExecute()
        );
    }
}
