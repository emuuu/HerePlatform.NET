namespace HerePlatformComponents.Maps.Coordinates;

/// <summary>
/// Represents a geographic point (H.geo.Point).
/// </summary>
public class GeoPoint
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public double? Alt { get; set; }

    public GeoPoint()
    {
    }

    public GeoPoint(double lat, double lng, double? alt = null)
    {
        Lat = lat;
        Lng = lng;
        Alt = alt;
    }
}
