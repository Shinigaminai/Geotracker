using System.Xml.Linq;
using NetTopologySuite.Geometries;

using Geotracker.Models;
using System.Diagnostics;

namespace Geotracker.Services;

public class TrailService
{
    public async Task<Trail> LoadTrailFromGpxStreamAsync(Stream stream)
    {

        // using var stream = await FileSystem.OpenAppPackageFileAsync(gpxFilePath);
        using var reader = new StreamReader(stream);

        var trail_xml_text = await reader.ReadToEndAsync();
        trail_xml_text = trail_xml_text.TrimStart('\uFEFF', '\u200B', ' ', '\n', '\r', '\t');
        Console.WriteLine(trail_xml_text);

        var xdoc = XDocument.Parse(trail_xml_text);
        var ns = xdoc.Root?.Name.Namespace;

        var trailname = xdoc.Descendants(ns + "name").First().Value;
        var FirstGpsPoint = xdoc.Descendants(ns + "trkpt").First();
        var FirstGpsLat = double.Parse(FirstGpsPoint.Attribute("lat")!.Value, System.Globalization.CultureInfo.InvariantCulture);
        var FirstGpsLon = double.Parse(FirstGpsPoint.Attribute("lat")!.Value, System.Globalization.CultureInfo.InvariantCulture);
        string location = await GetGeocodeReverseLocation(FirstGpsLat, FirstGpsLon);
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
            Location = location,
            Coordinates = linePoints
        };
    }

    public async Task<FileResult?> PickGpxFile()
    {
        try
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "com.topografix.gpx" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/gpx+xml", "application/octet-stream" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".gpx" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "gpx" } }, // UTType values
                });

            PickOptions options = new()
            {
                PickerTitle = "Please select a comic file",
                FileTypes = customFileType,
            };
            return await FilePicker.Default.PickAsync(options);


            // return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
            Debug.WriteLine(ex.Message);
        }

        return null;
    }
    private async Task<string> GetGeocodeReverseLocation(double latitude, double longitude)
    {
        IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

        Placemark? placemark = placemarks?.FirstOrDefault();

        if (placemark != null)
        {
            string[] locData = { placemark.CountryCode, placemark.Locality, placemark.SubLocality };
            locData = [.. locData.Where(c => c != null)];
            return String.Join(" - ", locData);
            // $"AdminArea:       {placemark.AdminArea}\n" +
            // $"CountryCode:     {placemark.CountryCode}\n" +
            // $"CountryName:     {placemark.CountryName}\n" +
            // $"FeatureName:     {placemark.FeatureName}\n" +
            // $"Locality:        {placemark.Locality}\n" +
            // $"PostalCode:      {placemark.PostalCode}\n" +
            // $"SubAdminArea:    {placemark.SubAdminArea}\n" +
            // $"SubLocality:     {placemark.SubLocality}\n" +
            // $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
            // $"Thoroughfare:    {placemark.Thoroughfare}\n";
        }

        return "";
    }
}
