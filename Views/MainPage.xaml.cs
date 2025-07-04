namespace Geotracker;

using Geotracker.ViewModels;
using Geotracker.Models;
using Mapsui.UI.Maui;
using System.Diagnostics;
using NetTopologySuite.Geometries;
using Mapsui.Nts.Extensions;
using Mapsui;
using System.Threading.Tasks;

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
			TrailMap.Map.Navigator.RotationLock = true;
		}
	}

	private async Task AddTrailAsync()
	{
		// select trail and load
		var file = await viewModel.ChooseGPXFile();
		// maybe user cancelled?
		if (file != null)
		{
			Trail trail = await viewModel.LoadTrailAsync(file);
			viewModel.AddTrailToMap(trail, TrailMap);
			ZoomToTrails();
		}
	}

	private void OnAddTrailClicked(object sender, EventArgs e)
	{
		_ = AddTrailAsync();
	}

	private void OnDeleteTrailInvoked(object sender, EventArgs e)
	{
		if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is TrailItem trailToDelete)
		{
			TrailMap.Map.Layers.Remove(trailToDelete.Layer);
			viewModel.TrailItems.Remove(trailToDelete);
		}
	}

	private void OnZoomToTrailInvoked(object sender, EventArgs e)
	{
		if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is TrailItem trailItem)
		{
			ZoomToTrail(trailItem);
		}
	}

	private void ZoomToTrail(TrailItem trailItem)
	{
		ZoomToArea(trailItem.Envelope);
	}

	private void ZoomToTrails()
	{
		// Zoom to fit all trails
		var newEnvelope = new Envelope();

		foreach (TrailItem TrailItem in viewModel.TrailItems)
		{
			newEnvelope = newEnvelope.ExpandedBy(TrailItem.Envelope);
			Debug.WriteLine("Expand area for trail \"" + TrailItem.Trail.Name + "\"");
			Debug.WriteLine("Area: " + newEnvelope.Area.ToString() + " at " + newEnvelope.Centre.ToString());
		}
		ZoomToArea(newEnvelope);
	}

	private void ZoomToArea(Envelope envelope)
	{
		envelope.ExpandBy(150.0);
		TrailMap.Map.Navigator.ZoomToBox(envelope.ToMRect(), MBoxFit.Fit);
	}
}
