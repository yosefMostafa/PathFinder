using Microsoft.Maui.Handlers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using ProjectFinder.Models;
using ProjectFinder.Services.windows;

namespace ProjectFinder.Views.Custom;

public partial class CustomFlexLayout : ContentView
{
	public CustomFlexLayout()
	{
		InitializeComponent();

	}
	private void OnItemFrameLoaded(object sender, EventArgs e)
	{
#if WINDOWS
		if (sender is VerticalStackLayout frame && frame.BindingContext is FileItem fileItem)
		{
			if (frame.Handler != null)
			{
				AttachPointerPressed(frame, fileItem);
			}
			else
			{
				// Ensure handler is available before accessing it
				frame.HandlerChanged += (s, e) => AttachPointerPressed(frame, fileItem);
			}
		}
#endif
	}

	private void AttachPointerPressed(VerticalStackLayout frame, FileItem fileItem)
	{
		var handler = frame.Handler as LayoutHandler;

		var nativeView = handler?.PlatformView as Microsoft.UI.Xaml.FrameworkElement;

		if (nativeView == null)
		{
			Console.WriteLine("Native view is null");
			return;
		}



		nativeView.PointerPressed += (s, args) =>
		{
			var point = args.GetCurrentPoint(nativeView);
			if (point.Properties.IsRightButtonPressed)
			{
				// 2. Get top-level window offset

				Microsoft.UI.Windowing.AppWindow? appWindow = WindowsServicesHandler.GetAppWindow();

				if (appWindow == null)
				{
					Console.WriteLine("AppWindow is null, cannot get position.");
					return;
				}

				var visualRoot = nativeView.XamlRoot.Content;
				var transformToRoot = nativeView.TransformToVisual(visualRoot);
				var relativeToRoot = transformToRoot.TransformPoint(point.Position);
				var windowPos = appWindow.Position;
				// 3. Add offset to get real screen coords
				double screenX = windowPos.X + relativeToRoot.X;
				double screenY = windowPos.Y + relativeToRoot.Y;

				if (BindingContext is MainPageViewModel vm &&
					vm.RightClickCommand?.CanExecute((fileItem, screenX, screenY)) == true)
				{
					vm.RightClickCommand.Execute((fileItem, screenX, screenY));
				}
			}
		};
	}
}