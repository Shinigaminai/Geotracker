namespace Geotracker;

using Geotracker.ViewModels;
using Geotracker.Models;

using Mapsui.UI.Maui;
using System.Diagnostics;

public partial class MainPage : ContentPage
{
	int count = 0;

	private double _startY;
	public double DrawerHeight { get; set; }
	public double DrawerTranslationY { get; set; }
	private bool _isOpen = false;

	private MainPageViewModel viewModel;

	public MainPage()
	{
		InitializeComponent();
		viewModel = new MainPageViewModel();
		BindingContext = viewModel;

		if (TrailMap.Map is not null)
		{
			TrailMap.Map.CRS = "EPSG:3857";
			TrailMap.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
		}
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		// await viewModel.LoadTrailAsync(TrailMap);

		var screenHeight = DeviceDisplay.MainDisplayInfo.Height;
		var screenDensity = DeviceDisplay.MainDisplayInfo.Density;
		screenHeight = screenHeight / screenDensity; // Use DIPs

		// TODO Calculate the usable screen height (to exclude the status and navigation bars)

		// DrawerHeight = screenHeight * 0.5;  // 50% of screen height
		// DrawerTranslationY = DrawerHeight - AddTrailBtn.Height - 50 - 20;

		// BottomDrawer.HeightRequest = DrawerHeight;
		// BottomDrawer.TranslationY = DrawerTranslationY;

		// Debug.WriteLine($"Drawer Height: {DrawerHeight}");
	}

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);

		// Recalculate drawer height when the size of the screen changes (e.g., rotation)
		if (width > 0 && height > 0)
		{
			DrawerHeight = height * 0.5;
			BottomDrawer.HeightRequest = DrawerHeight;
			DrawerTranslationY = DrawerContentLayout.Height +
								DrawerContentLayout.Margin.Top +
								DrawerContentLayout.Margin.Bottom +
								DrawerContentLayout.Padding.Top +
								DrawerContentLayout.Padding.Bottom;
		}
		Debug.WriteLine($"New Drawer Height: {DrawerHeight} and max translation {DrawerTranslationY}");
	}


	void OnDrawerPanUpdated(object sender, PanUpdatedEventArgs e)
	{
		switch (e.StatusType)
		{
			case GestureStatus.Started:
				_startY = BottomDrawer.TranslationY;
				break;

			case GestureStatus.Running:
				var newY = _startY + e.TotalY;
				BottomDrawer.TranslationY = Math.Max(0, Math.Min(DrawerTranslationY, newY));
				Debug.WriteLine($"Drawer Y: {e.TotalY} / {DrawerTranslationY}");
				break;

			case GestureStatus.Completed:
				// Animate open/close based on where it stopped
				if (BottomDrawer.TranslationY < DrawerTranslationY / 2)
				{
					OpenDrawer();
				}
				else
				{
					CloseDrawer();
				}
				break;
		}
	}

	private async void OpenDrawer()
	{
		_isOpen = true;
		await BottomDrawer.TranslateTo(0, 0, 250, Easing.CubicOut);
	}

	private async void CloseDrawer()
	{
		_isOpen = false;
		await BottomDrawer.TranslateTo(0, DrawerTranslationY, 250, Easing.CubicIn);
	}

	private void OnAddTrailClicked(object sender, EventArgs e)
	{
		// select trail and load
		_ = viewModel.LoadTrailAsync(TrailMap);
	}
}
