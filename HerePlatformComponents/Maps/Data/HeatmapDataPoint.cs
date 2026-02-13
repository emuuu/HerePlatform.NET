namespace HerePlatformComponents.Maps.Data;

/// <summary>
/// A data point for heatmap visualization.
/// </summary>
public class HeatmapDataPoint
{
    /// <summary>
    /// Latitude.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Longitude.
    /// </summary>
    public double Lng { get; set; }

    /// <summary>
    /// Intensity value for the heatmap.
    /// </summary>
    public double Value { get; set; } = 1;

    public HeatmapDataPoint() { }

    public HeatmapDataPoint(double lat, double lng, double value = 1)
    {
        Lat = lat;
        Lng = lng;
        Value = value;
    }
}
