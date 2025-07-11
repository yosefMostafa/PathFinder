using System;
using Microsoft.UI.Windowing;
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
    public static void CloseMenuDelegate()
    {
        CloseContextMenu();
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
    public static void ToggleFullScreenAsync()
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
            case FullScreenPresenter:
                appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
                break;

            case OverlappedPresenter:
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                break;

            default:
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                break;
        }

        // Optional delay for layout settle

    }
}
