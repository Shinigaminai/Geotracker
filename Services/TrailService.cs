using System.Xml.Linq;
using NetTopologySuite.Geometries;

using Geotracker.Models;

namespace Geotracker.Services;

public class TrailService
{
    public async Task<Trail> LoadTrailFromGpxAsync(string gpxFilePath)
    {

        using var stream = await FileSystem.OpenAppPackageFileAsync(gpxFilePath);
        using var reader = new StreamReader(stream);

        var trail_xml_text = await reader.ReadToEndAsync();
        trail_xml_text = trail_xml_text.TrimStart('\uFEFF', '\u200B', ' ', '\n', '\r', '\t');
        Console.WriteLine(trail_xml_text);

        var xdoc = XDocument.Parse(trail_xml_text);
        var ns = xdoc.Root?.Name.Namespace;

        var trailname = xdoc.Descendants(ns + "name").First().Value;
        var linePoints = xdoc.Descendants(ns + "trkpt")
            .Select(pt =>
            {
                double lat = double.Parse(pt.Attribute("lat")!.Value, System.Globalization.CultureInfo.InvariantCulture);
                double lon = double.Parse(pt.Attribute("lon")!.Value, System.Globalization.CultureInfo.InvariantCulture);
                // Mapsui uses lon/lat (x/y) -> transform to spherical mercator for osm
                // var convPoint = SphericalMercator.FromLonLat(new MPoint(lon, lat));
                return new Coordinate(lon, lat);
            })
            .ToArray();

        // var trackLine = new LineString(linePoints);
        // var traillocation = Mapsui.

        return new Geotracker.Models.Trail
        {
            Name = trailname,
            Location = "TBD",
            Coordinates = linePoints
        };
    }
}
