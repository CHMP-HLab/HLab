using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MantisBTRestAPIClient;


namespace HLab.Bugs.Wpf
{
    /// <summary>
    /// Logique d'interaction pour Exception.xaml
    /// </summary>
    public partial class ExceptionView : Window
    {
        Exception _exception;

        public ExceptionView()
        {
            InitializeComponent();
            SetLanguage();

            CommentTextBox.Focus();
            CommentTextBox.SelectAll();

            if (Debugger.IsAttached)
            {
                ThrowButton.Visibility = Visibility.Visible;
                ReportCheckBox.IsChecked = false;
            }
        }

        public Exception Exception
        {
            get => _exception;
            set
            {
                _exception = value;
                ErrorMessageTextBlock.Text = Exception.Message;
            }
        }

        public string Token { get; set; }
        public string Url { get; set; }
        public string Project { get; set; }


        void SetLanguage()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            if (currentCulture.Name == "fr-FR")
            {
                Title = "Rapport d'anomalie";
                MessageTextBlock.Text = "Arret impromptu de l'application";
                SubMessageTextBlock.Text =
                    "Cliquez sur relancer pour rouvrir l'application.";
                ReportCheckBox.Content = "Envoyer un rapport";
                CommentTextBlock.Text = "Informations complèmentaires :";
                CommentTextBox.Text = "Pas de commentaire";
                ShowDetailButton.Content = "Détail";
                OkButton.Content = "Ok";
                ReopenButton.Content = "Relancer";
            }
        }


        void ShowDetailButton_OnChecked(object sender, RoutedEventArgs e)
        {
            DetailTextBlock.Text = Exception.ToString();
            ScrollViewer.Visibility = Visibility.Visible;
            DetailTextBlock.Visibility = Visibility.Visible;
        }

        void ShowDetailButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ScrollViewer.Visibility = Visibility.Collapsed;
            DetailTextBlock.Visibility = Visibility.Collapsed;
        }

        Task Post() => PostGithub();

        void PostMantis()
        {
            if (ReportCheckBox.IsChecked != true) return;
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
                Description = CommentTextBox.Text, 
                Additional_information = Exception.StackTrace,
                
            };

            client.IssueAdd(issue);
        }


        async Task PostGithub()
        {
            if (ReportCheckBox.IsChecked != true) return;
            if (string.IsNullOrWhiteSpace(Token)) return;

            var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("hlab.chmp.org"));

            var tokenAuth = new Octokit.Credentials(Token); 
            client.Credentials = tokenAuth;

            var user = await client.User.Current();

            var createIssue = new Octokit.NewIssue("[" + Exception.Source + "]" +Exception.Message)
            {
                Body = $"{CommentTextBox.Text}\n{Exception.StackTrace}", 
            };

            var issue = await client.Issue.Create("CHMP-HLab", "HLab.Erp.Lims.Analysis", createIssue);
        }

        async void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Post();
            Application.Current.Shutdown();
        }

        async void ReopenButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Post();
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        void ReportCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (CommentTextBox == null) return;
            CommentTextBlock.Visibility = Visibility.Visible;
            CommentTextBox.Visibility = Visibility.Visible;
        }

        void ReportCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (CommentTextBox == null) return;
            CommentTextBlock.Visibility = Visibility.Collapsed;
            CommentTextBox.Visibility = Visibility.Collapsed;
        }

        void ThrowButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
