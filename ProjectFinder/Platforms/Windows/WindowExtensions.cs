#if WINDOWS
using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using ProjectFinder.Services.Windows;

public static class FullscreenHelper
{
    private const int WM_KEYDOWN = 0x0100;
    private const int VK_F11 = 0x7A;
    private static IntPtr _hwnd;
    private static bool _isFullscreen = false;
    private static ShellContextMenuHelper.WINDOWPLACEMENT _previousPlacement;

    private static ShellContextMenuHelper.SUBCLASSPROC _proc = SubclassProc;

    public static void EnableF11Fullscreen(Microsoft.UI.Xaml.Window window)
    {
       _hwnd = WindowNative.GetWindowHandle(window);
    Console.WriteLine("Subclassing window: " + _hwnd);

    // bool result = SetWindowSubclass(_hwnd, _proc, IntPtr.Zero, IntPtr.Zero);
    // Console.WriteLine("Subclass result: " + result);
    }

    private static IntPtr SubclassProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
    {
        if (msg == WM_KEYDOWN && wParam.ToInt32() == VK_F11)
        {
            ToggleFullscreen();
            return IntPtr.Zero;
        }

        return ShellContextMenuHelper.DefSubclassProc(hWnd, msg, wParam, lParam);
    }

    public static void ToggleFullscreen()
    {
        if (!_isFullscreen)
        {
            GetWindowPlacement(_hwnd, out _previousPlacement);
            SetWindowLong(_hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
            ShowWindow(_hwnd, SW_SHOWMAXIMIZED);
        }
        else
        {
            SetWindowLong(_hwnd, GWL_STYLE, WS_OVERLAPPEDWINDOW | WS_VISIBLE);
            SetWindowPlacement(_hwnd, ref _previousPlacement);
        }

        _isFullscreen = !_isFullscreen;
    }

    // Win32 constants and imports
    private const int GWL_STYLE = -16;
    private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
    private const int WS_VISIBLE = 0x10000000;
    private const int WS_POPUP = unchecked((int)0x80000000);
    private const int SW_SHOWMAXIMIZED = 3;

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(IntPtr hWnd, out ProjectFinder.Services.Windows.ShellContextMenuHelper.WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref ProjectFinder.Services.Windows.ShellContextMenuHelper.WINDOWPLACEMENT lpwndpl);

    [DllImport("comctl32.dll", SetLastError = true)]
    private static extern bool SetWindowSubclass(IntPtr hWnd, ProjectFinder.Services.Windows.ShellContextMenuHelper.SUBCLASSPROC pfnSubclass, IntPtr uIdSubclass, IntPtr dwRefData);
}
#endif
