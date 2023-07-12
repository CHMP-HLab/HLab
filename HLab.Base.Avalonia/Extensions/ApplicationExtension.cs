using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace HLab.Base.Avalonia.Extensions;

public static class ApplicationExtension
{
    public static TopLevel? GetTopLevel(this Application app)
    {
        switch (app.ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime { MainWindow: {} w }:
                return w;

            case IClassicDesktopStyleApplicationLifetime { Windows.Count: > 0 } desktop:
                return desktop.Windows[0];

            case ISingleViewApplicationLifetime viewApp:
            {
                var visualRoot = viewApp.MainView?.GetVisualRoot();
                return visualRoot as TopLevel;
            }
            default:
                return null;
        }
    }
}