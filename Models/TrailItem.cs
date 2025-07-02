using Geotracker.Models;
using Mapsui.Layers;

namespace Geotracker.Models
{
    public class TrailItem
    {
        public required Trail Trail { get; set; }
        public string ImageUrl { get; set; } = "path.svg";
        public required MemoryLayer Layer { get; set; }
        public Mapsui.Styles.Color Color { get; set; } = Mapsui.Styles.Color.Red;
    }
}
