using System.Collections.ObjectModel;
using System.ComponentModel;
using Geotracker.Models;
using Geotracker.Services;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using Mapsui.UI.Maui;
using Mapsui.Layers;
using System.Runtime.CompilerServices;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui;
using Mapsui.Nts.Extensions;

namespace Geotracker.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly TrailService _trailService;
    public ObservableCollection<Trail> Trails { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    private List<Mapsui.Styles.Color> TrailColors = new List<Mapsui.Styles.Color>
    {
        Mapsui.Styles.Color.Red,
        Mapsui.Styles.Color.Blue,
        Mapsui.Styles.Color.LightCoral,
        Mapsui.Styles.Color.DarkCyan,
        Mapsui.Styles.Color.Salmon
    };

    public MainPageViewModel()
    {
        Trails = new ObservableCollection<Trail> { };
        _trailService = new TrailService();
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task LoadTrailAsync(MapControl map)
    {
        var file = await _trailService.PickGpxFile();
        if (file == null)
        {
            // maybe user cancelled?
            return;
        }
        var fileStream = await file.OpenReadAsync();
        var trail = await _trailService.LoadTrailFromGpxStreamAsync(fileStream);

        var mercatorCoords = trail.Coordinates
            .Select(c => SphericalMercator.FromLonLat(c.X, c.Y))
            .Select(p => new Coordinate(p.x, p.y))
            .ToArray();

        var geometryFactory = new GeometryFactory();
        var geometryLine = geometryFactory.CreateLineString(mercatorCoords);
        var trailFeature = new GeometryFeature { Geometry = geometryLine };

        var nrOfColors = TrailColors.Count;
        var nrOfTrailLayers = Trails.Count;
        var color = TrailColors[nrOfTrailLayers % nrOfColors];
        trail.Color = color;

        trailFeature.Styles.Add(new VectorStyle
        {
            Line = new Pen(color, 3)
        });

        var layer = new MemoryLayer
        {
            Name = "Trail-" + trail.Name,
            Features = new[] { trailFeature }
        };
        trail.Layer = layer;
        map.Map.Layers.Add(layer);
        Trails.Add(trail);

        // Zoom to trail
        var mrect = geometryLine.EnvelopeInternal.ToMRect();
        map.Map.Navigator.ZoomToBox(mrect, MBoxFit.Fit);
    }
}
