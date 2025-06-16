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

    public MainPageViewModel()
    {
        Trails = new ObservableCollection<Trail>
            {
                new Trail { Name = "Forest Path", Location = "Blackwood", ImageUrl = "path.svg" },
                new Trail { Name = "Mountain Trail", Location = "Alpine Ridge", ImageUrl = "path.svg" },
                new Trail { Name = "River Walk", Location = "Blue River", ImageUrl = "path.svg" }
            };
        _trailService = new TrailService();
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task LoadTrailAsync(MapControl map)
    {
        var trail = await _trailService.LoadTrailFromGpxAsync("mytrack.gpx");

        var mercatorCoords = trail.Coordinates
            .Select(c => SphericalMercator.FromLonLat(c.X, c.Y))
            .Select(p => new Coordinate(p.x, p.y))
            .ToArray();

        var geometryFactory = new GeometryFactory();
        var geometryLine = geometryFactory.CreateLineString(mercatorCoords);
        var trailFeature = new GeometryFeature { Geometry = geometryLine };

        trailFeature.Styles.Add(new VectorStyle
        {
            Line = new Pen(Mapsui.Styles.Color.Red, 3)
        });

        var layer = new MemoryLayer
        {
            Name = "Trail-" + trail.Name,
            Features = new[] { trailFeature }
        };

        map.Map.Layers.Add(layer);

        // Zoom to trail
        var mrect = geometryLine.EnvelopeInternal.ToMRect();
        map.Map.Navigator.ZoomToBox(mrect, MBoxFit.Fit);
    }
}
