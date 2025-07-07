namespace ProjectFinder;

using Microsoft.Maui.Handlers;
using ProjectFinder.Models;



public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		
		BindingContext = new MainPageViewModel();

	}

	private void OnItemGridLoaded(object sender, EventArgs e)
	{
#if WINDOWS
		if (sender is Grid grid && grid.BindingContext is FileItem fileItem)
		{
			var handler = grid.Handler as LayoutHandler;
			var nativeView = handler?.PlatformView as Microsoft.UI.Xaml.FrameworkElement;

			if (nativeView != null)
			{
				nativeView.PointerPressed += (s, args) =>
				{
					var point = args.GetCurrentPoint(nativeView);
					if (args.GetCurrentPoint(nativeView).Properties.IsRightButtonPressed)
					{
						double x = point.Position.X;
						double y = point.Position.Y;

						// Convert to screen coordinates
						var screenPoint = nativeView.TransformToVisual(null)
													.TransformPoint(point.Position);
						// 🔁 Access ViewModel and invoke command
						if (BindingContext is MainPageViewModel vm &&
					vm.RightClickCommand?.CanExecute((fileItem, screenPoint.X, screenPoint.Y)) == true)
						{
							vm.RightClickCommand.Execute((fileItem, screenPoint.X, screenPoint.Y));
						}
					}
				};
			}
		}
#endif
	}

}

