namespace Geotracker;

using Geotracker.ViewModels;


public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();

		TrailMap.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
	}
}
