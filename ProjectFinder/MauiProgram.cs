using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;

namespace ProjectFinder;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
			#if WINDOWS

       builder.ConfigureLifecycleEvents(events =>  
        {  
            events.AddWindows(wndLifeCycleBuilder =>  
            {  
                wndLifeCycleBuilder.OnWindowCreated(window =>  
                {  
                    window.ExtendsContentIntoTitleBar = false;  
                    IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);  
                    WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);  
                    var _appWindow = AppWindow.GetFromWindowId(myWndId);  
					// FullscreenHelper.EnableF11Fullscreen(_appWindow);
                    _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);                          
                });  
            });  
        });  
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

