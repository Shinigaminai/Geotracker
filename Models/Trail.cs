using NetTopologySuite.Geometries;
using Mapsui.Layers;

namespace Geotracker.Models
{
    public class Trail
    {
        public required string Name { get; set; }
        public string? Location { get; set; }
        public string? Type { get; set; }
        public string ImageUrl { get; set; } = "path.svg";

        // List of GPS Points (lon/lat)
        public Coordinate[] Coordinates { get; set; } = [];
        public MemoryLayer? Layer { get; set; }
        public Mapsui.Styles.Color Color { get; set; } = Mapsui.Styles.Color.Red;
    }
}
