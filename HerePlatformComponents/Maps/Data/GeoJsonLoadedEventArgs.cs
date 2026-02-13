namespace HerePlatformComponents.Maps.Data;

/// <summary>
/// Event data when GeoJSON data has been loaded.
/// </summary>
public class GeoJsonLoadedEventArgs
{
    /// <summary>
    /// Number of objects loaded from the GeoJSON data.
    /// </summary>
    public int ObjectCount { get; set; }
}
