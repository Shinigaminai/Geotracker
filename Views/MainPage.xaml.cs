namespace Geotracker;

using Geotracker.ViewModels;
using Geotracker.Models;
using Mapsui.UI.Maui;
using System.Diagnostics;

public partial class MainPage : ContentPage
{
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

	private void OnAddTrailClicked(object sender, EventArgs e)
	{
		// select trail and load
		_ = viewModel.LoadTrailAsync(TrailMap);
	}

	private void OnDeleteTrailInvoked(object sender, EventArgs e)
	{
		if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is TrailItem trailToDelete)
		{
			TrailMap.Map.Layers.Remove(trailToDelete.Layer);
			viewModel.TrailItems.Remove(trailToDelete);
		}
	}
}
