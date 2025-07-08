using System;
using ProjectFinder.Services.Windows;

namespace ProjectFinder.Services.windows;

public class WindowsServicesHandler : ShellContextMenuHelper
{
    public static void ShowMenuDelegate(string path, double x, double y)
    {
        if (string.IsNullOrEmpty(path))
        {
            Console.WriteLine("Path is null or empty, cannot show context menu.");
            return;
        }

        Show(path, (int)x, (int)y);

    }
    public static Microsoft.UI.Windowing.AppWindow? GetAppWindow()
    {
        if (GetParentWindow() == null)
        {
            Console.WriteLine("Parent window is null, cannot get AppWindow.");
            return null;
        }
        else
        {

            return GetAppWindow(GetParentWindow()!);
        }
    }
    public static void ToggleFullScreen()
    {
        MauiWinUIWindow? window = GetParentWindow();
        if (window == null)
        {
            Console.WriteLine("Window is null, cannot toggle fullscreen.");
            return;
        }
        var appWindow = GetAppWindow(window);

        switch (appWindow.Presenter)
        {
            case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                if (overlappedPresenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Maximized)
                {
                    overlappedPresenter.SetBorderAndTitleBar(true, true);
                    overlappedPresenter.Restore();
                }
                else
                {
                    overlappedPresenter.SetBorderAndTitleBar(false, false);
                    overlappedPresenter.Maximize();
                }

                break;

        }
    }
}
