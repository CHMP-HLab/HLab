using System;
using System.Windows;
using HLab.Core;

namespace HLab.Mvvm
{
    public class DialogService : Service, IDialogService
    {
        bool? ShowMessage(string text, string caption, MessageBoxButton button, string icon)
        {
            if (!Enum.TryParse("Active", out MessageBoxImage img))
                img = MessageBoxImage.Information;

            var result = MessageBox.Show(text,caption,MessageBoxButton.OK, img);
            switch (result)
            {
                case MessageBoxResult.None:
                    return null;
                case MessageBoxResult.OK:
                    return true;
                case MessageBoxResult.Cancel:
                    return null;
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }

        public void ShowMessageOk(string text, string caption, string icon)
            => ShowMessage(text, caption, MessageBoxButton.OK, icon);

        public bool ShowMessageOkCancel(string text, string caption, string icon)
            => ShowMessage(text, caption, MessageBoxButton.OKCancel, icon)??false;

        public bool ShowMessageYesNo(string text, string caption, string icon)
            => ShowMessage(text, caption, MessageBoxButton.YesNo, icon) ?? false;

        public bool? ShowMessageYesNoCancel(string text, string caption, string icon)
            => ShowMessage(text, caption, MessageBoxButton.YesNoCancel, icon);
    }
}
