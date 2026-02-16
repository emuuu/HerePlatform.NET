using System;

namespace HerePlatform.Core.Coordinates;

/// <summary>
/// Represents a geographic point (H.geo.Point).
/// </summary>
public class GeoPoint
{
    private double _lat;
    private double _lng;

    public double Lat
    {
        get => _lat;
        set
        {
            if (value is < -90 or > 90)
                throw new ArgumentOutOfRangeException(nameof(value), "Latitude values can only range from -90 to 90.");
            _lat = value;
        }
    }

    public double Lng
    {
        get => _lng;
        set
        {
            if (value is < -180 or > 180)
                throw new ArgumentOutOfRangeException(nameof(value), "Longitude values can only range from -180 to 180.");
            _lng = value;
        }
    }

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
