namespace Geotracker.Models
{
    public class Trail
    {
        public required string Name { get; set; }
        public string? Location { get; set; }
        public string ImageUrl { get; set; } = "path.svg";
    }
}
