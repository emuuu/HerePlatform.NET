namespace HerePlatformComponents.Maps.Clustering;

/// <summary>
/// Event data for cluster or noise point tap.
/// </summary>
public class ClusterTapEventArgs
{
    /// <summary>
    /// Geographic position of the cluster/point.
    /// </summary>
    public LatLngLiteral? Position { get; set; }

    /// <summary>
    /// Total weight of the cluster (1 for noise points).
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Number of data points in the cluster (1 for noise points).
    /// </summary>
    public int PointCount { get; set; }

    /// <summary>
    /// Whether this is a cluster (true) or a noise/single point (false).
    /// </summary>
    public bool IsCluster { get; set; }
}
