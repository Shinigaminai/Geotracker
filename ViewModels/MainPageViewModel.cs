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
using System.Diagnostics;
using System.Threading.Tasks;

namespace Geotracker.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly TrailService _trailService;
    public ObservableCollection<TrailItem> TrailItems { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    private List<Microsoft.Maui.Graphics.Color> TrailColors = new List<Microsoft.Maui.Graphics.Color>
    {
        Colors.Red,
        Colors.LightCoral,
        Colors.Teal,
        Colors.Blue,
        Colors.DeepPink,
        Colors.DarkCyan,
        Colors.Purple
    };

    public MainPageViewModel()
    {
        TrailItems = new ObservableCollection<TrailItem> { };
        _trailService = new TrailService();
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Task<FileResult?> ChooseGPXFile()
    {
        return _trailService.PickGpxFile();
    }

    public async Task<Trail> LoadTrailAsync(FileResult file)
    {
        var fileStream = await file.OpenReadAsync();
        return await _trailService.LoadTrailFromGpxStreamAsync(fileStream);
    }

    public void AddTrailToMap(Trail trail, MapControl map)
    {
        var mercatorCoords = trail.Coordinates
            .Select(c => SphericalMercator.FromLonLat(c.X, c.Y))
            .Select(p => new Coordinate(p.x, p.y))
            .ToArray();

        var geometryFactory = new GeometryFactory();
        var geometryLine = geometryFactory.CreateLineString(mercatorCoords);
        var trailFeature = new GeometryFeature { Geometry = geometryLine };

        var nrOfColors = TrailColors.Count;
        var nrOfTrailLayers = TrailItems.Count;
        var color = TrailColors[nrOfTrailLayers % nrOfColors];

        Mapsui.Styles.Color mapsuiColor = Mapsui.Styles.Color.FromString(color.ToArgbHex());
        trailFeature.Styles.Add(new VectorStyle
        {
            Line = new Pen(mapsuiColor, 3)
        });

        var layer = new MemoryLayer
        {
            Name = "Trail-" + trail.Name,
            Features = new[] { trailFeature }
        };
        map.Map.Layers.Add(layer);

        TrailItem trailItem = new TrailItem
        {
            Color = color,
            Layer = layer,
            Trail = trail,
            Envelope = geometryLine.EnvelopeInternal
        };
        TrailItems.Add(trailItem);
    }
}
