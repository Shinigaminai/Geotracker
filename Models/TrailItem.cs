using Geotracker.Models;
using Mapsui.Layers;
using NetTopologySuite.Geometries;

namespace Geotracker.Models
{
    public class TrailItem
    {
        public required Trail Trail { get; set; }
        public string ImageUrl { get; set; } = "path.svg";
        public required MemoryLayer Layer { get; set; }
        public Color Color { get; set; } = Colors.Red;
        public required Envelope Envelope { get; set; }
    }
}
