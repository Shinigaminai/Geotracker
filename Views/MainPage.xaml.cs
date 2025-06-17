namespace Geotracker;

using Geotracker.ViewModels;
using Geotracker.Models;

using Mapsui.UI.Maui;

public partial class MainPage : ContentPage
{
	int count = 0;

	private double _startY;
	private bool _isOpen = false;

	private MainPageViewModel viewModel;

	public MainPage()
	{
		InitializeComponent();
		viewModel = new MainPageViewModel();

		if (TrailMap.Map is not null)
		{
			TrailMap.Map.CRS = "EPSG:3857";
			TrailMap.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
		}
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await viewModel.LoadTrailAsync(TrailMap);
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
				BottomDrawer.TranslationY = Math.Max(0, Math.Min(250, newY));
				break;

			case GestureStatus.Completed:
				// Animate open/close based on where it stopped
				if (BottomDrawer.TranslationY < 125)
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
		await BottomDrawer.TranslateTo(0, 250, 250, Easing.CubicIn);
	}
}
