namespace Geotracker;

using Geotracker.ViewModels;
using Geotracker.Models;

using Mapsui.UI.Maui;
using System.Diagnostics;

public partial class MainPage : ContentPage
{
	int count = 0;

	private double _startY;
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

	// protected override async void OnAppearing()
	// {
	// 	base.OnAppearing();
	// 	// await viewModel.LoadTrailAsync(TrailMap);
	// }


	void OnDrawerPanUpdated(object sender, PanUpdatedEventArgs e)
	{
		switch (e.StatusType)
		{
			case GestureStatus.Started:
				_startY = BottomDrawer.TranslationY;
				DrawerTranslationY = TrailsCollection.Height +
							TrailsCollection.Margin.Top +
							TrailsCollection.Margin.Bottom; ;
				Debug.WriteLine($"Set max drawer translation to {DrawerTranslationY}");
				break;

			case GestureStatus.Running:
				var newY = _startY + e.TotalY;
				BottomDrawer.TranslationY = Math.Max(0, Math.Min(DrawerTranslationY, newY));
				Debug.WriteLine($"Drawer Y: {BottomDrawer.TranslationY} / {DrawerTranslationY}");
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
