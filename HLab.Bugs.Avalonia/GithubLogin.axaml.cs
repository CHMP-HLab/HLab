using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HLab.Bugs.Avalonia
{
    /// <summary>
    /// Logique d'interaction pour GithubLogin.xaml
    /// </summary>
    public partial class GithubLogin : UserControl
    {
        public GithubLogin()
        {
            InitializeComponent();
        }

        void PasswordBox_OnTextChanged(object? sender, TextChangedEventArgs e)
        {
        }
    }
}
