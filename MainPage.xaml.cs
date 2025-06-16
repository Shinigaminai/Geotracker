namespace Geotracker;

using Geotracker.ViewModels;
using System.Xml.Linq;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Providers;
using NetTopologySuite.Geometries;

using Mapsui.UI.Maui;
using Mapsui;
using NetTopologySuite.Features;
using Mapsui.Nts.Extensions;

using Mapsui.Nts;
using Geotracker.Models;
using Mapsui.Projections;

public partial class MainPage : ContentPage
{
	int count = 0;

	private double _startY;
	private bool _isOpen = false;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();

		if (TrailMap.Map is not null)
		{
			TrailMap.Map.CRS = "EPSG:3857";
			TrailMap.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_ = LoadGpx("It_s_T_raining_men_.gpx", TrailMap); // Called when the UI is ready
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

	public async Task LoadGpx(string gpxFilePath, MapControl mapControl)
	{
		using var stream = await FileSystem.OpenAppPackageFileAsync(gpxFilePath);
		using var reader = new StreamReader(stream);

		var trail_xml_text = await reader.ReadToEndAsync();
		trail_xml_text = trail_xml_text.TrimStart('\uFEFF', '\u200B', ' ', '\n', '\r', '\t');
		Console.WriteLine(trail_xml_text);

		var xdoc = XDocument.Parse(trail_xml_text);
		var ns = xdoc.Root?.Name.Namespace;

		var trackname = xdoc.Descendants(ns + "name").First().Value;
		var linePoints = xdoc.Descendants(ns + "trkpt")
			.Select(pt =>
			{
				double lat = double.Parse(pt.Attribute("lat")!.Value, System.Globalization.CultureInfo.InvariantCulture);
				double lon = double.Parse(pt.Attribute("lon")!.Value, System.Globalization.CultureInfo.InvariantCulture);
				// Mapsui uses lon/lat (x/y) -> transform to spherical mercator for osm
				var convPoint = SphericalMercator.FromLonLat(new MPoint(lon, lat));
				return new Coordinate(convPoint.X, convPoint.Y);
			})
			.ToArray();

		var trackLine = new LineString(linePoints);

		var trackFeature = new GeometryFeature { Geometry = trackLine };
		trackFeature.Styles.Add(new VectorStyle
		{
			Line = new Pen(Color.Red, 3)
		});

		var trackLayer = new MemoryLayer
		{
			Name = trackname,
			Features = new[] { trackFeature }
		};

		mapControl.Map.Layers.Add(trackLayer);

		mapControl.Map.Navigator.ZoomToBox(trackLine.EnvelopeInternal.ToMRect(), MBoxFit.Fit);
	}

}
