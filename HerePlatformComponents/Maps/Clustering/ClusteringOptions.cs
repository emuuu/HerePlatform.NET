namespace HerePlatformComponents.Maps.Clustering;

/// <summary>
/// Options for marker clustering behavior.
/// </summary>
public class ClusteringOptions
{
    /// <summary>
    /// Epsilon parameter for DBSCAN clustering. Controls cluster radius.
    /// Default: 32.
    /// </summary>
    public double Eps { get; set; } = 32;

    /// <summary>
    /// Minimum weight for a cluster to be formed.
    /// Default: 2.
    /// </summary>
    public int MinWeight { get; set; } = 2;

    /// <summary>
    /// Clustering strategy.
    /// </summary>
    public ClusteringStrategy Strategy { get; set; } = ClusteringStrategy.FastGrid;
}

/// <summary>
/// Clustering algorithm strategy.
/// </summary>
public enum ClusteringStrategy
{
    FastGrid,
    Grid,
    DynamicGrid
}
