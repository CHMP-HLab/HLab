using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MantisBTRestAPIClient;

namespace HLab.Erp.Lims.Monographs.Loader
{
    /// <summary>
    /// Logique d'interaction pour Exception.xaml
    /// </summary>
    public partial class ExceptionView : Window
    {
        public ExceptionView()
        {
            InitializeComponent();
            SetLanguage();

            txtComment.Focus();
            txtComment.SelectAll();
        }

        public Exception Exception { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public string Project { get; set; }


        private void SetLanguage()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;
            if (currentCulture.Name == "fr-FR")
            {
                Title = "Rapport d'anomalie";
                tbMessage.Text = "Arret impromptu de l'application";
                tbSubMessage.Text =
                    "Cliquez sur relancer pour rouvrir l'application.";
                chkReport.Content = "Envoyer un rapport";
                tbComment.Text = "Informations complèmentaires :";
                txtComment.Text = "Pas de commentaire";
                ButtonShowDetail.Content = "Détail";
                ButtonOk.Content = "Ok";
                ButtonReopen.Content = "Relancer";
            }
        }


        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            txtDetail.Text = Exception.ToString();
            ScrollViewer.Visibility = Visibility.Visible;
            txtDetail.Visibility = Visibility.Visible;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ScrollViewer.Visibility = Visibility.Collapsed;
            txtDetail.Visibility = Visibility.Collapsed;
        }


        private void Post()
        {
            if (chkReport.IsChecked != true) return;
            if (string.IsNullOrWhiteSpace(Token)) return;

            var httpClient = new HttpClient();
            var client = MantisHTTPClientFactory.New(
                Url ,
                Token,
                httpClient
                );

            var user = client.UserGetMe();

            var account = new AccountRef
            {
                Name = user.Name,
                Id = user.Id,
                Email = user.Email
            };

            var projectRef = user.Projects.FirstOrDefault(p => p.Name == Project);
            if (projectRef == null) return;

            var project = new Identifier
            {
                Id = projectRef.Id,
                Name = projectRef.Name,
            };

            var category = new Identifier
            {
                Id = 0,
                Name = "General"
            };

            var issue = new Issue
            {
                Summary = "[" + Exception.Source + "]" +Exception.Message, 
                Reporter = account,
                Project = project,
                Category = category,
                Description = txtComment.Text, 
                Additional_information = Exception.StackTrace,
                
            };

            client.IssueAdd(issue);
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            Post();
            Application.Current.Shutdown();
        }

        private void ButtonReopen_OnClick(object sender, RoutedEventArgs e)
        {
            Post();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void ChkReport_OnChecked(object sender, RoutedEventArgs e)
        {
            if (txtComment == null) return;
            txtComment.Visibility = Visibility.Visible;
            tbComment.Visibility = Visibility.Visible;
        }

        private void ChkReport_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (txtComment == null) return;
            txtComment.Visibility = Visibility.Collapsed;
            tbComment.Visibility = Visibility.Collapsed;
        }
    }
}
