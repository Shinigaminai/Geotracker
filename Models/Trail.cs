using NetTopologySuite.Geometries;

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
    }
}
