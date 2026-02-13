namespace HerePlatformComponents.Maps.Clustering;

/// <summary>
/// A data point for marker clustering.
/// </summary>
public class ClusterDataPoint
{
    /// <summary>
    /// Latitude of the data point.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Longitude of the data point.
    /// </summary>
    public double Lng { get; set; }

    /// <summary>
    /// Weight of the data point for clustering aggregation.
    /// </summary>
    public int Weight { get; set; } = 1;

    /// <summary>
    /// Optional arbitrary data attached to this point.
    /// </summary>
    public object? Data { get; set; }

    public ClusterDataPoint() { }

    public ClusterDataPoint(double lat, double lng, int weight = 1, object? data = null)
    {
        Lat = lat;
        Lng = lng;
        Weight = weight;
        Data = data;
    }
}
