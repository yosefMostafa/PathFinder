
// #if WINDOWS
// using Microsoft.UI.Xaml;

// namespace ProjectFinder;

// public partial class WinApp : MauiWinUIApplication
// {
//     protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

//     protected override void OnLaunched(LaunchActivatedEventArgs args)
//     {
//         base.OnLaunched(args);

//         var nativeWindow = Microsoft.Maui.Controls.Application.Current?.Windows
//             .FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

//         if (nativeWindow != null)
//         {
//             FullscreenHelper.EnableF11Fullscreen(nativeWindow);
//             nativeWindow.CoreWindow.KeyDown += (s, e) =>
//             {
//                 if (e.VirtualKey == Windows.System.VirtualKey.F11)
//                 {
//                     FullscreenHelper.ToggleFullscreen();
//                 }
//             };
//         }
//     }
// }

// #endif